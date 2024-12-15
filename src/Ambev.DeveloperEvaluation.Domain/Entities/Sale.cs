using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public new Guid Id { get; set; } = Guid.NewGuid();
    public required string SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public required string Customer { get; set; }
    public decimal TotalAmount { get; set; }
    public required string Branch { get; set; }
    public List<SaleItem> Items { get; set; } = new List<SaleItem>();
    public bool IsCancelled { get; set; } = false;

    public void CalculateTotal()
    {
        TotalAmount = 0;
        foreach (var item in Items)
        {
            item.ApplyDiscount();
            TotalAmount += item.Total;
        }
    }
}
public class SaleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal Total { get; set; }

    public void ApplyDiscount()
    {
        if (Quantity >= 4 && Quantity < 10)
        {
            Discount = 0.10m;
        }
        else if (Quantity >= 10 && Quantity <= 20)
        {
            Discount = 0.20m;
        }
        else if (Quantity > 20)
        {
            throw new InvalidOperationException("Cannot sell more than 20 identical items.");
        }

        Total = Quantity * UnitPrice * (1 - Discount);
    }
}