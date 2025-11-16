using DeliFHery.Api.Dtos;
using DeliFHery.Domain;

namespace DeliFHery.Api.Mappers;

public static class DeliveryMapper
{
    public static DeliveryDto ToDto(this DeliveryOrder deliveryOrder)
    {
        ArgumentNullException.ThrowIfNull(deliveryOrder);

        return new DeliveryDto(
            deliveryOrder.Id,
            deliveryOrder.OrderNumber,
            deliveryOrder.ScheduledAt,
            deliveryOrder.DeliveredAt,
            deliveryOrder.Status,
            deliveryOrder.CustomerId);
    }

    public static DeliveryOrder ToDomain(this DeliveryRequest request, Guid deliveryId, Guid customerId)
    {
        ArgumentNullException.ThrowIfNull(request);

        var deliveryOrder = new DeliveryOrder(
            deliveryId,
            request.OrderNumber!,
            request.ScheduledAt,
            request.Status,
            customerId)
        {
            DeliveredAt = request.DeliveredAt
        };

        return deliveryOrder;
    }
}
