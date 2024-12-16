using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM;

public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<MongoSettings> settings, IConfiguration configuration)
    {
        // Obter a string de conexão da seção "ConnectionStrings:MongoDB"
        var connectionString = configuration.GetConnectionString("MongoDB");
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "A string de conexão do MongoDB não pode ser nula.");

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}