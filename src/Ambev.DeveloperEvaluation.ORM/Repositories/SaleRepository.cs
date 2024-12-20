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
            .Include(s => s.Items) 
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Update(Sale sale)
    {
        _context.Attach(sale);
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
            Console.WriteLine("Concurrency conflict detected: " + ex.Message);
            throw new InvalidOperationException("Concurrency conflict: The sale was modified or deleted by another process.", ex);
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

    public void RemoveAllItems(Sale sale)
    {
        _context.Entry(sale).Collection(s => s.Items).Load(); 
        _context.SaleItems.RemoveRange(sale.Items); 
    }

   public void AddRangeItems(IEnumerable<SaleItem> items)
    {
        _context.SaleItems.AddRange(items);
    }
    public void AttachAsModified<T>(T entity) where T : class
    {
        _context.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
}