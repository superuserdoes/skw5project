using DeliFHery.Domain;

namespace DeliFHery.Logic;

internal class DeliveryOrderValidator : IEntityValidator<DeliveryOrder>
{
    public void ValidateAndThrow(DeliveryOrder entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (string.IsNullOrWhiteSpace(entity.OrderNumber))
        {
            throw new ArgumentException("Order number must be provided", nameof(entity));
        }

        if (entity.ScheduledAt == default)
        {
            throw new ArgumentException("Scheduled date must be set", nameof(entity));
        }

        if (entity.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id must be provided", nameof(entity));
        }

        if (entity.WeightKg <= 0)
        {
            throw new ArgumentException("Weight must be greater than zero", nameof(entity));
        }

        if (entity.LengthCm <= 0 || entity.WidthCm <= 0 || entity.HeightCm <= 0)
        {
            throw new ArgumentException("Dimensions must be greater than zero", nameof(entity));
        }

        if (string.IsNullOrWhiteSpace(entity.OriginPostalCode) || string.IsNullOrWhiteSpace(entity.DestinationPostalCode))
        {
            throw new ArgumentException("Origin and destination postal codes must be provided", nameof(entity));
        }
    }
}
