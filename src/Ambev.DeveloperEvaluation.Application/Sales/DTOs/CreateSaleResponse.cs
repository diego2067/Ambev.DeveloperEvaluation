namespace Ambev.DeveloperEvaluation.Application.Sales.DTOs;

public class CreateSaleResponse
{
    public Guid Id { get; set; }
    public required string SaleNumber { get; set; }
    public required string Customer { get; set; }
    public required string Branch { get; set; }
    public required decimal TotalAmount { get; set; }
    public required string RowVersion { get; set; } 
}