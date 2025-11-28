using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing;

public interface IPriceRule
{
    void Apply(DeliveryOrder order, PriceCalculationContext context);
}
