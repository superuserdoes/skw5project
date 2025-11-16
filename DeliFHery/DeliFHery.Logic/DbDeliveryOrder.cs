using DeliFHery.Domain;

namespace DeliFHery.Logic;

internal class DbDeliveryOrder
{
    public DbDeliveryOrder(Guid id, string orderNumber, DateTimeOffset scheduledAt, DateTimeOffset? deliveredAt, DeliveryStatus status, Guid customerId)
    {
        Id = id;
        OrderNumber = orderNumber;
        ScheduledAt = scheduledAt;
        DeliveredAt = deliveredAt;
        Status = status;
        CustomerId = customerId;
    }

    public Guid Id { get; set; }
    public string OrderNumber { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DeliveryStatus Status { get; set; }
    public Guid CustomerId { get; set; }

    public DeliveryOrder ToDomain() => new DeliveryOrder(Id, OrderNumber, ScheduledAt, Status, CustomerId)
    {
        DeliveredAt = DeliveredAt
    };
}

internal static class DeliveryOrderMappingExtensions
{
    public static DbDeliveryOrder ToDbDelivery(this DeliveryOrder delivery) => new(
        delivery.Id,
        delivery.OrderNumber,
        delivery.ScheduledAt,
        delivery.DeliveredAt,
        delivery.Status,
        delivery.CustomerId);
}
