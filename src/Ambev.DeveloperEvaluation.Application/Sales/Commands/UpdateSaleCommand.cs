using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands;

public class UpdateSaleCommand : IRequest<bool>
{
    public Guid SaleId { get; }
    public UpdateSaleRequest Request { get; }
    public byte[] RowVersion { get; set; }

    public UpdateSaleCommand(Guid saleId, UpdateSaleRequest request)
    {
        SaleId = saleId;
        Request = request;
        RowVersion = Convert.FromBase64String(request.RowVersion);
    }
}