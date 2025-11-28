using System.Collections.Generic;
using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

/// <summary>
/// Applies month-based percentage multipliers to the subtotal (base + distance).
/// Discounts are represented as negative values so peak-season reductions (e.g., December)
/// lower the quoted price rather than inflating it.
/// </summary>
public class SeasonalAdjustmentRule : IPriceRule
{
    /// <summary>
    /// Month number to seasonal adjustment multiplier. Negative entries represent discounts,
    /// e.g., -0.10 for a 10% reduction during December promotions. Extend or adjust this map
    /// to reflect promotional or peak-period pricing.
    /// </summary>
    public static readonly IReadOnlyDictionary<int, decimal> MonthlyMultipliers = new Dictionary<int, decimal>
    {
        [12] = -0.10m
    };

    public void Apply(DeliveryOrder order, PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(context);

        var subtotal = context.BasePrice + context.DistanceSurcharge;
        if (subtotal <= 0)
        {
            return;
        }

        MonthlyMultipliers.TryGetValue(order.ScheduledAt.Month, out var multiplier);

        if (multiplier <= 0)
        {
            return;
        }

        context.AddSeasonalAdjustment(subtotal * multiplier);
    }
}
