using DeliFHery.Api.Dtos;
using DeliFHery.Domain;
using DeliFHery.Logic.Pricing;

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
            deliveryOrder.CustomerId,
            deliveryOrder.WeightKg,
            deliveryOrder.LengthCm,
            deliveryOrder.WidthCm,
            deliveryOrder.HeightCm,
            deliveryOrder.OriginPostalCode,
            deliveryOrder.DestinationPostalCode,
            deliveryOrder.BasePrice,
            deliveryOrder.DistanceSurcharge,
            deliveryOrder.SeasonalAdjustment,
            deliveryOrder.TotalPrice);
    }

    public static DeliveryOrder ToDomain(this DeliveryRequest request, Guid deliveryId, Guid customerId)
    {
        ArgumentNullException.ThrowIfNull(request);

        var deliveryOrder = new DeliveryOrder(
            deliveryId,
            request.OrderNumber!,
            request.ScheduledAt,
            request.Status,
            customerId,
            request.WeightKg,
            request.LengthCm,
            request.WidthCm,
            request.HeightCm,
            request.OriginPostalCode,
            request.DestinationPostalCode)
        {
            DeliveredAt = request.DeliveredAt
        };

        return deliveryOrder;
    }

    public static PriceQuoteDto ToDto(this PriceBreakdown breakdown)
    {
        ArgumentNullException.ThrowIfNull(breakdown);
        return new PriceQuoteDto(
            breakdown.BasePrice,
            breakdown.DistanceSurcharge,
            breakdown.SeasonalAdjustment,
            breakdown.TotalPrice);
    }
}
