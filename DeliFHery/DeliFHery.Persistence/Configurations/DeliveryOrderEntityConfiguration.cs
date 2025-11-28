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

        builder.Property(d => d.WeightKg)
            .HasPrecision(9, 2)
            .IsRequired();

        builder.Property(d => d.LengthCm)
            .HasPrecision(9, 2)
            .IsRequired();

        builder.Property(d => d.WidthCm)
            .HasPrecision(9, 2)
            .IsRequired();

        builder.Property(d => d.HeightCm)
            .HasPrecision(9, 2)
            .IsRequired();

        builder.Property(d => d.OriginPostalCode)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(d => d.DestinationPostalCode)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(d => d.BasePrice).HasPrecision(10, 2);
        builder.Property(d => d.DistanceSurcharge).HasPrecision(10, 2);
        builder.Property(d => d.SeasonalAdjustment).HasPrecision(10, 2);
        builder.Property(d => d.TotalPrice).HasPrecision(10, 2);

        builder.HasData(
            new DeliveryOrderEntity
            {
                Id = SeedData.OrchardMorningDeliveryId,
                OrderNumber = "ORD-1000",
                ScheduledAt = SeedData.OrchardMorningSchedule,
                Status = DeliveryStatus.Created,
                CustomerId = SeedData.OrchardMarketCustomerId,
                WeightKg = 2.5m,
                LengthCm = 30,
                WidthCm = 20,
                HeightCm = 10,
                OriginPostalCode = "94103",
                DestinationPostalCode = "94107",
                BasePrice = 10m,
                DistanceSurcharge = 2.5m,
                SeasonalAdjustment = 0,
                TotalPrice = 12.5m
            },
            new DeliveryOrderEntity
            {
                Id = SeedData.CityLunchDeliveryId,
                OrderNumber = "ORD-2000",
                ScheduledAt = SeedData.CityLunchSchedule,
                Status = DeliveryStatus.Created,
                CustomerId = SeedData.CityBakeryCustomerId,
                WeightKg = 6.2m,
                LengthCm = 40,
                WidthCm = 25,
                HeightCm = 15,
                OriginPostalCode = "94016",
                DestinationPostalCode = "94501",
                BasePrice = 15m,
                DistanceSurcharge = 7.5m,
                SeasonalAdjustment = 0,
                TotalPrice = 22.5m
            });
    }
}
