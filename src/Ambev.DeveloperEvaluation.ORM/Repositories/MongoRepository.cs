using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class MongoRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoContext context, IOptions<MongoSettings> settings)
    {
       _collection = context.GetCollection<T>(settings.Value.CollectionName);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("_id", id.ToString());
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task InsertAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        
        var idProperty = entity.GetType().GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(entity, id);
        }
       
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq("Id", id)
        );

        
        var updateResult = await _collection.ReplaceOneAsync(filter, entity);

        
        if (updateResult.MatchedCount == 0)
        {
            var currentDocument = await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync();

            if (currentDocument == null)
                throw new InvalidOperationException("The sale was deleted by another process.");

            throw new InvalidOperationException("The sale was updated by another process. Please reload the data.");
        }
    }

    public async Task UpdatePartialAsync(Guid id, UpdateDefinition<T> updateDefinition)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.UpdateOneAsync(filter, updateDefinition);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.CountDocumentsAsync(filter);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        var count = await _collection.CountDocumentsAsync(filter);
        return count > 0;
    }
}