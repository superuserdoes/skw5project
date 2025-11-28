using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeliFHery.Domain;

namespace DeliFHery.Persistence.Entities;

[Table("delivery_orders", Schema = "delifhery")]
public class DeliveryOrderEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string OrderNumber { get; set; } = string.Empty;

    public DateTimeOffset ScheduledAt { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }

    public DeliveryStatus Status { get; set; }

    public Guid CustomerId { get; set; }

    public decimal WeightKg { get; set; }

    public decimal LengthCm { get; set; }

    public decimal WidthCm { get; set; }

    public decimal HeightCm { get; set; }

    [MaxLength(32)]
    public string OriginPostalCode { get; set; } = string.Empty;

    [MaxLength(32)]
    public string DestinationPostalCode { get; set; } = string.Empty;

    [Column(TypeName = "numeric(10,2)")]
    public decimal BasePrice { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal DistanceSurcharge { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal SeasonalAdjustment { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal TotalPrice { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public CustomerEntity? Customer { get; set; }
}
