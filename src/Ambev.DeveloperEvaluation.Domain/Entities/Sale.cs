using Ambev.DeveloperEvaluation.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public new Guid Id { get; set; } = Guid.NewGuid();

    public string SaleNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public string Customer { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Branch { get; set; } = string.Empty;

    public List<SaleItem> Items { get; set; } = new List<SaleItem>();

    public bool IsCancelled { get; set; } = false;

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public void CalculateTotal()
    {
        TotalAmount = Items.Sum(item =>
        {
            item.ApplyDiscount();
            return item.Total;
        });
    }
}

public class SaleItem
{
    public Guid SaleId { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; } = 0;

    public decimal Total { get; set; }

    public void ApplyDiscount()
    {
        Discount = Quantity switch
        {
            >= 4 and < 10 => 0.10m,
            >= 10 and <= 20 => 0.20m,
            > 20 => throw new InvalidOperationException("Cannot sell more than 20 identical items."),
            _ => 0m
        };

        Total = Quantity * UnitPrice * (1 - Discount);
    }
}