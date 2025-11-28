using System.Linq;
using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

/// <summary>
/// Computes a base price using the greater of physical and dimensional weight and configurable brackets.
/// </summary>
public class WeightPriceRule : IPriceRule
{
    /// <summary>
    /// Divisor used to convert cubic centimeters into kilograms for dimensional weight (industry standard: 5000).
    /// </summary>
    public const decimal VolumetricDivisor = 5000m;

    /// <summary>
    /// Inclusive weight thresholds (in kg) and their corresponding base prices (in the local currency).
    /// Adjust these pairs to tune the stepped pricing curve without modifying rule code.
    /// </summary>
    public static readonly (decimal MaxWeightKg, decimal Price)[] WeightBrackets =
    {
        (1m, 5m),
        (5m, 10m),
        (10m, 15m),
        (20m, 25m)
    };

    /// <summary>
    /// Flat base price applied when a parcel exceeds the highest configured weight bracket.
    /// </summary>
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
