using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM;

public class DefaultContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<SaleMongo>  SaleMongoItem{ get; set; }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.RowVersion)
                  .IsRowVersion()
                  .IsConcurrencyToken();

            entity.HasMany(s => s.Items)
      .            WithOne()
                   .HasForeignKey(i => i.SaleId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Product).IsRequired();
        });

        modelBuilder.Entity<SaleMongo>(entity =>
        {
            entity.HasKey(e => e.Id); 
            entity.ToTable("SalesCollection");

            modelBuilder.Entity<SaleMongoItem>().HasNoKey();
        });

        base.OnModelCreating(modelBuilder);

    }
}
public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("PostgreSQL");



        builder.UseNpgsql(
               connectionString,
               b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
        );

        return new DefaultContext(builder.Options);
    }
}