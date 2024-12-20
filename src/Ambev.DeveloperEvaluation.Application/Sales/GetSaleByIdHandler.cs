using Ambev.DeveloperEvaluation.Application.Sales.Querys;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, Sale>
{
    private readonly SaleRepository _sqlRepository;

    public GetSaleByIdHandler(SaleRepository sqlRepository)
    {
        _sqlRepository = sqlRepository;
    }

    public async Task<Sale> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken)
    {
        return await _sqlRepository.GetByIdAsync(query.SaleId);
    }
}