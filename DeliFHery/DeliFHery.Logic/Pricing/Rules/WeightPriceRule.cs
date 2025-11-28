using System.Linq;
using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

/// <summary>
/// Computes a base price using the greater of physical and dimensional weight and configurable brackets.
/// </summary>
public class WeightPriceRule : IPriceRule
{
    public const decimal VolumetricDivisor = 5000m;

    /// <summary>
    /// Weight breakpoints (inclusive) and their corresponding prices. Adjust the values to tweak the
    /// stepped base-pricing strategy without touching rule logic.
    /// </summary>
    public static readonly (decimal MaxWeightKg, decimal Price)[] WeightBrackets =
    {
        (1m, 5m),
        (5m, 10m),
        (10m, 15m),
        (20m, 25m)
    };

    public const decimal OversizePrice = 40m;

    public void Apply(DeliveryOrder order, PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(context);

        var dimensionalWeight = CalculateDimensionalWeight(order);
        var chargeableWeight = Math.Max(order.WeightKg, dimensionalWeight);

        var basePrice = WeightBrackets.FirstOrDefault(b => chargeableWeight <= b.MaxWeightKg).Price;
        if (basePrice == default)
        {
            basePrice = OversizePrice;
        }

        context.SetBasePrice(basePrice);
    }

    private static decimal CalculateDimensionalWeight(DeliveryOrder order)
    {
        var volume = order.LengthCm * order.WidthCm * order.HeightCm;
        return Math.Round(volume / VolumetricDivisor, 2, MidpointRounding.AwayFromZero);
    }
}
