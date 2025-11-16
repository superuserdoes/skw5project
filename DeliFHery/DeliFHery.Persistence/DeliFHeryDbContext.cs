using DeliFHery.Persistence.Configurations;
using DeliFHery.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliFHery.Persistence;

public class DeliFHeryDbContext(DbContextOptions<DeliFHeryDbContext> options) : DbContext(options)
{
    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();

    public DbSet<DeliveryOrderEntity> DeliveryOrders => Set<DeliveryOrderEntity>();

    public DbSet<ContactEntity> Contacts => Set<ContactEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryOrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContactEntityConfiguration());
    }
}
