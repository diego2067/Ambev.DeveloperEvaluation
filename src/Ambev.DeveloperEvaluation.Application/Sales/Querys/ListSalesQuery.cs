using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Querys;

public class ListSalesQuery : IRequest<IEnumerable<Sale>>
{
    public int PageNumber { get; }
    public int PageSize { get; }

    public ListSalesQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}