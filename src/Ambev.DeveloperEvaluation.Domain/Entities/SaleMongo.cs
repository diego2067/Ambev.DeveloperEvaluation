using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleMongo
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [BsonElement("saleNumber")]
    public string SaleNumber { get; set; } = string.Empty;

    [BsonElement("saleDate")]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    [BsonElement("customer")]
    public string Customer { get; set; } = string.Empty;

    [BsonElement("branch")]
    public string Branch { get; set; } = string.Empty;

    [BsonElement("totalAmount")]
    public decimal TotalAmount { get; set; }

    [NotMapped]
    [BsonElement("items")]
    public List<SaleMongoItem> Items { get; set; } = new();

    [BsonElement("isCancelled")]
    public bool IsCancelled { get; set; } = false;
}