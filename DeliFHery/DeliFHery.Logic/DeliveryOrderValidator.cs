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
    }
}
