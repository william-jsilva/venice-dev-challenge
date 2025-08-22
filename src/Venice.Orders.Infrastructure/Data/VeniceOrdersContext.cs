using Microsoft.EntityFrameworkCore;
using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Infrastructure.Data;

public class VeniceOrdersContext : DbContext
{
    public VeniceOrdersContext(DbContextOptions<VeniceOrdersContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            
            // Ignorar a propriedade Items pois será salva no MongoDB
            entity.Ignore(e => e.Items);
        });
    }
}

