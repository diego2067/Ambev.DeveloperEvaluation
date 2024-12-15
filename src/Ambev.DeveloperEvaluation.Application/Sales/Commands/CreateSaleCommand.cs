using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands;

public class CreateSaleCommand : IRequest<Guid>
{
    public CreateSaleRequest Request { get; }

    public CreateSaleCommand(CreateSaleRequest request)
    {
        Request = request;
    }
}