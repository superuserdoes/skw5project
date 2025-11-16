using DeliFHery.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliFHery.Persistence.Configurations;

internal class CustomerEntityConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Street).IsRequired().HasMaxLength(256);
        builder.Property(c => c.City).IsRequired().HasMaxLength(128);
        builder.Property(c => c.PostalCode).IsRequired().HasMaxLength(32);
        builder.Property(c => c.Notes).HasMaxLength(1024);

        builder.HasMany(c => c.Deliveries)
            .WithOne(d => d.Customer!)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Contacts)
            .WithOne(contact => contact.Customer!)
            .HasForeignKey(contact => contact.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new CustomerEntity
            {
                Id = SeedData.OrchardMarketCustomerId,
                Name = "Orchard Market",
                Street = "Sunrise Blvd 10",
                City = "Springfield",
                PostalCode = "10001",
                Notes = "Prefers early deliveries"
            },
            new CustomerEntity
            {
                Id = SeedData.CityBakeryCustomerId,
                Name = "City Bakery",
                Street = "Downtown Ave 5",
                City = "Springfield",
                PostalCode = "10002",
                Notes = "Call ahead when delayed"
            });
    }
}
