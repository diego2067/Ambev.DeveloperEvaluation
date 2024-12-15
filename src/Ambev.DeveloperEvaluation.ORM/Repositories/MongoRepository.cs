using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class MongoRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoContext context, string collectionName)
    {
        _collection = context.GetCollection<T>(collectionName);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", id), entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
    }
}