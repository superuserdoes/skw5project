using DeliFHery.Domain;
using DeliFHery.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliFHery.Persistence.Configurations;

internal class ContactEntityConfiguration : IEntityTypeConfiguration<ContactEntity>
{
    public void Configure(EntityTypeBuilder<ContactEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Type)
            .HasConversion(
                value => value.ToString(),
                value => Enum.Parse<ContactType>(value))
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(c => c.Value).IsRequired().HasMaxLength(256);

        builder.HasData(
            new ContactEntity
            {
                Id = SeedData.OrchardPrimaryContactId,
                CustomerId = SeedData.OrchardMarketCustomerId,
                Type = ContactType.Email,
                Value = "orders@orchard-market.example",
                IsPrimary = true
            },
            new ContactEntity
            {
                Id = SeedData.OrchardBackupContactId,
                CustomerId = SeedData.OrchardMarketCustomerId,
                Type = ContactType.Phone,
                Value = "+1-800-555-1234",
                IsPrimary = false
            },
            new ContactEntity
            {
                Id = SeedData.CityPrimaryContactId,
                CustomerId = SeedData.CityBakeryCustomerId,
                Type = ContactType.Phone,
                Value = "+1-800-555-9876",
                IsPrimary = true
            });
    }
}
