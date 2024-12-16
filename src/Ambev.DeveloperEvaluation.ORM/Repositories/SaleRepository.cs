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

    public void Update(Sale sale, byte[] rowVersion)
    {
        _context.Sales.Attach(sale);
        
        _context.Entry(sale).Property(s => s.RowVersion).OriginalValue = rowVersion;
        
        _context.Entry(sale).Property(s => s.Customer).IsModified = true;
        _context.Entry(sale).Property(s => s.Branch).IsModified = true;
        
        foreach (var item in sale.Items)
        {
            _context.Entry(item).State = item.Id == Guid.Empty ? EntityState.Added : EntityState.Modified;
        }
    }

    public async Task SaveChangesAsync()
    {
        bool saveFailed;
        do
        {
            saveFailed = false;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                saveFailed = true;

                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Sale)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        if (databaseValues == null)
                        {
                            throw new InvalidOperationException("The record no longer exists in the database.");
                        }

                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw new InvalidOperationException("Concurrency conflict detected on an unexpected entity.");
                    }
                }
            }
        } while (saveFailed);
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

}