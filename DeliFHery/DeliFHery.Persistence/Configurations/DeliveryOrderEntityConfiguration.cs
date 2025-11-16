using DeliFHery.Domain;
using DeliFHery.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliFHery.Persistence.Configurations;

internal class DeliveryOrderEntityConfiguration : IEntityTypeConfiguration<DeliveryOrderEntity>
{
    public void Configure(EntityTypeBuilder<DeliveryOrderEntity> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.OrderNumber).IsRequired().HasMaxLength(64);
        builder.HasIndex(d => d.OrderNumber).IsUnique();
        builder.Property(d => d.Status)
            .HasConversion(
                value => value.ToString(),
                value => Enum.Parse<DeliveryStatus>(value))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(d => d.ScheduledAt).IsRequired();

        builder.HasData(
            new DeliveryOrderEntity
            {
                Id = SeedData.OrchardMorningDeliveryId,
                OrderNumber = "ORD-1000",
                ScheduledAt = SeedData.OrchardMorningSchedule,
                Status = DeliveryStatus.Created,
                CustomerId = SeedData.OrchardMarketCustomerId
            },
            new DeliveryOrderEntity
            {
                Id = SeedData.CityLunchDeliveryId,
                OrderNumber = "ORD-2000",
                ScheduledAt = SeedData.CityLunchSchedule,
                Status = DeliveryStatus.Created,
                CustomerId = SeedData.CityBakeryCustomerId
            });
    }
}
