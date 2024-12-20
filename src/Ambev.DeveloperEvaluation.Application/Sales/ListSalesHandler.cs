using Ambev.DeveloperEvaluation.Application.Sales.Querys;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class ListSalesHandler : IRequestHandler<ListSalesQuery, IEnumerable<Sale>>
{
    private readonly SaleRepository _sqlRepository;

    public ListSalesHandler(SaleRepository sqlRepository)
    {
        _sqlRepository = sqlRepository;
    }

    public async Task<IEnumerable<Sale>> Handle(ListSalesQuery query, CancellationToken cancellationToken)
    {
        return await _sqlRepository.GetAllPagedAsync(query.PageNumber, query.PageSize);
    }
}