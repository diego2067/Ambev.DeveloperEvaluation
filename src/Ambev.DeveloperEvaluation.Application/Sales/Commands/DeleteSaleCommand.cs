using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands
{
    public class DeleteSaleCommand : IRequest<bool>
    {
        public Guid SaleId { get; }

        public DeleteSaleCommand(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}