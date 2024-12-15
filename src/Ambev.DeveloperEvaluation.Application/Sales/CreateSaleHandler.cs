using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly SaleRepository _sqlRepository;           
    private readonly SalesMongoRepository _mongoRepository;   

    public CreateSaleHandler(SaleRepository sqlRepository, SalesMongoRepository mongoRepository)
    {
        _sqlRepository = sqlRepository;
        _mongoRepository = mongoRepository;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
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

        await _mongoRepository.InsertAsync(sale);

        return sale.Id;
    }
}