using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task AddAsync(Sale sale);
    Task<Sale> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task<IEnumerable<Sale>> GetAllPagedAsync(int pageNumber, int pageSize);
}