namespace Ambev.DeveloperEvaluation.Application.Sales.DTOs;

public class CreateSaleRequest
{
    public required string SaleNumber { get; set; }
    public required string Customer { get; set; }
    public required string Branch { get; set; }
    public required List<CreateSaleItemRequest> Items { get; set; }
}

public class CreateSaleItemRequest
{
    public required string Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}