namespace DeliFHery.Domain;

public class DeliveryOrder
{
    public DeliveryOrder(
        Guid id,
        string orderNumber,
        DateTimeOffset scheduledAt,
        DeliveryStatus status,
        Guid customerId)
    {
        Id = id;
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        ScheduledAt = scheduledAt;
        Status = status;
        CustomerId = customerId;
    }

    public Guid Id { get; set; }

    public string OrderNumber { get; set; }

    public DateTimeOffset ScheduledAt { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }

    public DeliveryStatus Status { get; set; }

    public Guid CustomerId { get; set; }
}
