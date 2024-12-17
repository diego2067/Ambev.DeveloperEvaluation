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
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = request.Request.SaleNumber,
            SaleDate = DateTime.UtcNow,
            Customer = request.Request.Customer,
            Branch = request.Request.Branch,
            Items = request.Request.Items.Select(item => new SaleItem
            {
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        sale.CalculateTotal();

        await _sqlRepository.AddAsync(sale);
        await _sqlRepository.SaveChangesAsync();

        var rowVersionBytes = sale.RowVersion;

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
            IsCancelled = sale.IsCancelled,
            RowVersion = rowVersionBytes
        };

        await _mongoRepository.InsertAsync(mongoSale);

              Console.WriteLine("RowVersion atual no banco: " + Convert.ToBase64String(sale.RowVersion));
        var rowVersionBase64 = Convert.ToBase64String(sale.RowVersion);

        return new CreateSaleResponse
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            RowVersion = rowVersionBase64
        };
    }
}