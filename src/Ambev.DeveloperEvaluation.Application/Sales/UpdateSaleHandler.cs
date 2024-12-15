using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;

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
        if (sale == null) return false;

        sale.Customer = command.Request.Customer;
        sale.Branch = command.Request.Branch;
        sale.Items = command.Request.Items.Select(item => new SaleItem
        {
            Product = item.Product,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        }).ToList();

        sale.CalculateTotal();

        await _sqlRepository.SaveChangesAsync();

        await _mongoRepository.UpdateAsync(sale.Id, sale);

        return true;
    }
}