using System.Linq;
using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

/// <summary>
/// Adds a stepped surcharge based on the numeric gap between origin and destination postal codes.
/// </summary>
public class DistanceSurchargeRule : IPriceRule
{
    /// <summary>
    /// Maximum numeric characters considered from a postal code when estimating distance.
    /// </summary>
    public const int PostalDigitLimit = 5;

    /// <summary>
    /// Size of each billing step (in kilometers) used to batch surcharges.
    /// </summary>
    public const decimal KilometersPerStep = 50m;

    /// <summary>
    /// Heuristic distance (in kilometers) represented by a single unit difference in the postal prefix.
    /// </summary>
    public const decimal KilometersPerUnit = 10m;

    /// <summary>
    /// Surcharge added per distance step when the postal gap exceeds the configured thresholds.
    /// </summary>
    public const decimal StepSurcharge = 2.5m;

    public void Apply(DeliveryOrder order, PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(context);

        var originValue = ExtractNumericPrefix(order.OriginPostalCode);
        var destinationValue = ExtractNumericPrefix(order.DestinationPostalCode);
        var distanceKm = Math.Abs(originValue - destinationValue) * KilometersPerUnit;

        if (distanceKm <= 0)
        {
            return;
        }

        var surchargeSteps = Math.Ceiling(distanceKm / KilometersPerStep);
        var surcharge = surchargeSteps * StepSurcharge;
        context.AddDistanceSurcharge(surcharge);
    }

    private static int ExtractNumericPrefix(string code)
    {
        var digits = new string(code.Where(char.IsDigit).Take(PostalDigitLimit).ToArray());
        return int.TryParse(digits, out var value) ? value : 0;
    }
}
