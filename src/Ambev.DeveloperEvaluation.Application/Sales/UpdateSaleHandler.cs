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
        Console.WriteLine("Iniciando atualização da venda...");

        Sale updatedSale;
        try
        {
            updatedSale = await UpdateSQLAsync(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar o SQL: {ex.Message}");
            throw;
        }

        try
        {
            await UpdateMongoDBAsync(updatedSale);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar o MongoDB: {ex.Message}");
            throw;
        }

        Console.WriteLine("Atualização realizada com sucesso no SQL e MongoDB.");
        return true;
    }

    private async Task<Sale> UpdateSQLAsync(UpdateSaleCommand command)
    {
        var existingSale = await _sqlRepository.GetByIdAsync(command.SaleId)
                            ?? throw new InvalidOperationException("Venda não encontrada no SQL.");

        Console.WriteLine("Reatualizando as informações principais da venda...");

        existingSale.Customer = command.Request.Customer;
        existingSale.Branch = command.Request.Branch;

        
        _sqlRepository.RemoveAllItems(existingSale);

        var newItems = command.Request.Items.Select(item =>
        {
            var saleItem = new SaleItem
            {
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                SaleId = existingSale.Id 
            };

            saleItem.Discount = 0; 
            saleItem.Total = 0;    
            saleItem.ApplyDiscount(); 

            return saleItem;
        }).ToList();
        
        _sqlRepository.AddRangeItems(newItems);
       
        existingSale.Items = newItems; 
        existingSale.CalculateTotal();

        _sqlRepository.AttachAsModified(existingSale);

        try
        {
            await _sqlRepository.SaveChangesAsync();
            Console.WriteLine("SQL atualizado com sucesso.");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine("Conflito de concorrência detectado: " + ex.Message);
            throw new InvalidOperationException("A venda foi modificada ou deletada por outro processo.", ex);
        }

        return existingSale;
    }

    private async Task UpdateMongoDBAsync(Sale updatedSale)
    {
        var saleMongo = await _mongoRepository.GetByIdAsync(updatedSale.Id);

        if (saleMongo == null)
        {
            Console.WriteLine("Venda não encontrada no MongoDB.");
            return;
        }

        saleMongo.Customer = updatedSale.Customer;
        saleMongo.Branch = updatedSale.Branch;
        saleMongo.TotalAmount = updatedSale.TotalAmount;

        saleMongo.Items = updatedSale.Items.Select(item => new SaleMongoItem
        {
            Product = item.Product,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Discount = item.Discount, 
            Total = item.Total        
        }).ToList();

        await _mongoRepository.UpdateAsync(updatedSale.Id, saleMongo);
        Console.WriteLine("MongoDB atualizado com sucesso.");
    }
}