using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Querys;

public class GetSaleByIdQuery : IRequest<Sale>
{
    public Guid SaleId { get; }

    public GetSaleByIdQuery(Guid saleId)
    {
        SaleId = saleId;
    }
}
