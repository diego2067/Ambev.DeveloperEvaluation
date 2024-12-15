﻿namespace Ambev.DeveloperEvaluation.Application.Sales.DTOs;

public class UpdateSaleRequest
{
    public required string Customer { get; set; }
    public required string Branch { get; set; }
    public required List<UpdateSaleItemRequest> Items { get; set; }
}

public class UpdateSaleItemRequest
{
    public required string Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

