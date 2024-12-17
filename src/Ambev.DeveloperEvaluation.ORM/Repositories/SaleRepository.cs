using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Sale sale)
    {
        
        await _context.Sales.AddAsync(sale);
    }

    public async Task<Sale> GetByIdAsync(Guid id)
    {
        return await _context.Sales
            .AsTracking() 
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Update(Sale sale)
    {
        if (sale == null)
            throw new ArgumentNullException(nameof(sale), "Sale entity cannot be null.");

        _context.Entry(sale).State = EntityState.Modified;
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine("Concurrency conflict: " + ex.Message);
            throw; 
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving changes: " + ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Sale>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .OrderBy(s => s.SaleDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public void SetRowVersion(Sale sale, byte[] originalRowVersion)
    {
        _context.Entry(sale).Property(p => p.RowVersion).OriginalValue = originalRowVersion;
    }
}