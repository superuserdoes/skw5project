using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

public class WeightPriceRule : IPriceRule
{
    public void Apply(DeliveryOrder order, PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(context);

        var dimensionalWeight = CalculateDimensionalWeight(order);
        var chargeableWeight = Math.Max(order.WeightKg, dimensionalWeight);

        var basePrice = chargeableWeight switch
        {
            <= 1m => 5m,
            <= 5m => 10m,
            <= 10m => 15m,
            <= 20m => 25m,
            _ => 40m
        };

        context.SetBasePrice(basePrice);
    }

    private static decimal CalculateDimensionalWeight(DeliveryOrder order)
    {
        const decimal divisor = 5000m; // standard volumetric divisor for cm-based calculations
        var volume = order.LengthCm * order.WidthCm * order.HeightCm;
        return Math.Round(volume / divisor, 2, MidpointRounding.AwayFromZero);
    }
}
