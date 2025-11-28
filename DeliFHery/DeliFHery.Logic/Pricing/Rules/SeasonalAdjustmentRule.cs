using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

public class SeasonalAdjustmentRule : IPriceRule
{
    public void Apply(DeliveryOrder order, PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(context);

        var subtotal = context.BasePrice + context.DistanceSurcharge;
        if (subtotal <= 0)
        {
            return;
        }

        var multiplier = order.ScheduledAt.Month switch
        {
            12 => 0.15m,
            11 => 0.08m,
            6 or 7 => 0.05m,
            _ => 0m
        };

        if (multiplier <= 0)
        {
            return;
        }

        context.AddSeasonalAdjustment(subtotal * multiplier);
    }
}
