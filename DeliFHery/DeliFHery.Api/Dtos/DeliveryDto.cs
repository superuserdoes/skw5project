using System.ComponentModel.DataAnnotations;
using DeliFHery.Domain;

namespace DeliFHery.Api.Dtos;

public record DeliveryDto(
    Guid Id,
    string OrderNumber,
    DateTimeOffset ScheduledAt,
    DateTimeOffset? DeliveredAt,
    DeliveryStatus Status,
    Guid CustomerId);

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
}
