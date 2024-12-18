using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, bool>
{
    private readonly SaleRepository _sqlRepository;
    private readonly SalesMongoRepository _mongoRepository;

    public UpdateSaleHandler(SaleRepository sqlRepository, SalesMongoRepository mongoRepository)
    {
        _sqlRepository = sqlRepository;
        _mongoRepository = mongoRepository;
    }

    public async Task<bool> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _sqlRepository.GetByIdAsync(command.SaleId);
        if (sale == null)
            throw new InvalidOperationException("Sale not found.");


        if (command.RowVersion == null || command.RowVersion.Length == 0)
            throw new InvalidOperationException("RowVersion is required for concurrency check.");

        var rowVersionBytes = command.RowVersion;

        sale.Customer = command.Request.Customer;
        sale.Branch = command.Request.Branch;

        sale.Items = command.Request.Items.Select(item => new SaleItem
        {
            Product = item.Product,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        }).ToList();

        sale.CalculateTotal();

        try
        {
            _sqlRepository.Update(sale);
            await _sqlRepository.SaveChangesAsync();

            var saleMongo = await _mongoRepository.GetByIdAsync(command.SaleId);

            if (saleMongo == null) return false;

            saleMongo.Customer = command.Request.Customer;
            saleMongo.Branch = command.Request.Branch;

            saleMongo.Items = command.Request.Items.Select(item =>
            {
                var saleItem = new SaleMongoItem
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                saleItem.ApplyDiscount();
                return saleItem;
            }).ToList();

            saleMongo.TotalAmount = saleMongo.Items.Sum(i => i.Total);

            await _mongoRepository.UpdateAsync(sale.Id, saleMongo, Convert.ToBase64String(sale.RowVersion));

            _sqlRepository.SetRowVersion(sale, sale.RowVersion);
            Console.WriteLine("Atualização realizada com sucesso no SQL e MongoDB.");
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("A venda foi modificada ou removida por outro processo.");
            throw new InvalidOperationException("The sale was updated or deleted by another process.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a venda: {ex.Message}");
            throw;
        }
    }
}