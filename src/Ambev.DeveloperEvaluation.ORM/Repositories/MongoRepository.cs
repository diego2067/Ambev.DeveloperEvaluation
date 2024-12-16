using Ambev.DeveloperEvaluation.Domain.Entities;
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
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    // Corrigido: Insere um documento e inicializa RowVersion apenas se for SaleMongo
    public async Task InsertAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    // Atualiza um documento com controle de RowVersion
    public async Task UpdateAsync(Guid id, T entity, string rowVersion)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq("Id", id),
            Builders<T>.Filter.Eq("RowVersion", rowVersion)
        );

        var updateResult = await _collection.ReplaceOneAsync(filter, entity);

        if (updateResult.MatchedCount == 0)
            throw new DbUpdateConcurrencyException("Concurrency conflict detected.");
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