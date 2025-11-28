using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing;

/// <summary>
/// A single pricing rule that can mutate the <see cref="PriceCalculationContext"/> based on a
/// <see cref="DeliveryOrder"/>. Rules are executed in registration order to form a pipeline.
/// </summary>
public interface IPriceRule
{
    void Apply(DeliveryOrder order, PriceCalculationContext context);
}
