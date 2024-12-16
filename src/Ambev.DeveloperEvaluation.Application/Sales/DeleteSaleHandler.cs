using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, bool>
{
    private readonly SaleRepository _sqlRepository;
    private readonly SalesMongoRepository _mongoRepository;

    public DeleteSaleHandler(SaleRepository sqlRepository, SalesMongoRepository mongoRepository)
    {
        _sqlRepository = sqlRepository;
        _mongoRepository = mongoRepository;
    }

    public async Task<bool> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _sqlRepository.GetByIdAsync(command.SaleId);
        if (sale == null) return false;

        sale.IsCancelled = true;

        
        await _sqlRepository.SaveChangesAsync();


        await _mongoRepository.UpdateAsync(sale.Id, sale, sale.RowVersion);

        return true;
    }
}