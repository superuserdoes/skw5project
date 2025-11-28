using System.ComponentModel.DataAnnotations;
using DeliFHery.Domain;

namespace DeliFHery.Api.Dtos;

public record DeliveryDto(
    Guid Id,
    string OrderNumber,
    DateTimeOffset ScheduledAt,
    DateTimeOffset? DeliveredAt,
    DeliveryStatus Status,
    Guid CustomerId,
    decimal WeightKg,
    decimal LengthCm,
    decimal WidthCm,
    decimal HeightCm,
    string OriginPostalCode,
    string DestinationPostalCode,
    decimal BasePrice,
    decimal DistanceSurcharge,
    decimal SeasonalAdjustment,
    decimal TotalPrice);

public record PriceQuoteDto(
    decimal BasePrice,
    decimal DistanceSurcharge,
    decimal SeasonalAdjustment,
    decimal TotalPrice);

public class DeliveryRequest
{
    [Required]
    [StringLength(100)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset ScheduledAt { get; set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? DeliveredAt { get; set; }
        = null;

    [Required]
    public DeliveryStatus Status { get; set; }
        = DeliveryStatus.Created;

    [Required]
    [Range(0.01, 1000, ErrorMessage = "Weight must be positive")]
    public decimal WeightKg { get; set; }
        = 0.01m;

    [Required]
    [Range(0.1, 1000, ErrorMessage = "Length must be positive")]
    public decimal LengthCm { get; set; }
        = 0.1m;

    [Required]
    [Range(0.1, 1000, ErrorMessage = "Width must be positive")]
    public decimal WidthCm { get; set; }
        = 0.1m;

    [Required]
    [Range(0.1, 1000, ErrorMessage = "Height must be positive")]
    public decimal HeightCm { get; set; }
        = 0.1m;

    [Required]
    [StringLength(32)]
    public string OriginPostalCode { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string DestinationPostalCode { get; set; } = string.Empty;
}
