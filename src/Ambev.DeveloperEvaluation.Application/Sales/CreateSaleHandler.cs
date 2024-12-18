using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResponse>
{
    private readonly SaleRepository _sqlRepository;
    private readonly MongoRepository<SaleMongo> _mongoRepository;

    public CreateSaleHandler(SaleRepository sqlRepository, MongoRepository<SaleMongo> mongoRepository)
    {
        _sqlRepository = sqlRepository;
        _mongoRepository = mongoRepository;
    }

    public async Task<CreateSaleResponse> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        Sale sale;

        try
        {
            sale = await CreateSaleInSqlAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inserir no SQL Server: {ex.Message}");
            throw new InvalidOperationException("Falha ao criar a venda no SQL Server.", ex);
        }

        try
        {
            await CreateSaleInMongoAsync(sale);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inserir no MongoDB: {ex.Message}");
            throw new InvalidOperationException("Falha ao criar a venda no MongoDB. A operação no SQL foi bem-sucedida.", ex);
        }

        return new CreateSaleResponse
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount
        };
    }
    private async Task<Sale> CreateSaleInSqlAsync(CreateSaleCommand request)
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = request.Request.SaleNumber,
            SaleDate = DateTime.UtcNow,
            Customer = request.Request.Customer,
            Branch = request.Request.Branch,
            Items = request.Request.Items.Select(item =>
            {
                var saleItem = new SaleItem
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                saleItem.ApplyDiscount();
                return saleItem;
            }).ToList()
        };

        sale.CalculateTotal();

        await _sqlRepository.AddAsync(sale);
        await _sqlRepository.SaveChangesAsync();

        return sale;
    }

    private async Task CreateSaleInMongoAsync(Sale sale)
    {
        var mongoSale = new SaleMongo
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            Items = sale.Items.Select(item => new SaleMongoItem
            {
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                Total = item.Total
            }).ToList(),
            IsCancelled = sale.IsCancelled
        };

        await _mongoRepository.InsertAsync(mongoSale);
        Console.WriteLine("MongoDB inserido com sucesso.");
    }
}