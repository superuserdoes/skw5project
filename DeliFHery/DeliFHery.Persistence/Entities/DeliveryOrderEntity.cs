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

    [ForeignKey(nameof(CustomerId))]
    public CustomerEntity? Customer { get; set; }
}
