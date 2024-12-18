using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleMongoItem
{
    [BsonElement("product")]
    public string Product { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unitPrice")]
    public decimal UnitPrice { get; set; }

    [BsonElement("discount")]
    public decimal Discount { get;  set; }

    [BsonElement("total")]
    public decimal Total { get;  set; }

    [BsonElement("totalAmount")]
    public decimal TotalAmount { get; set; }
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

