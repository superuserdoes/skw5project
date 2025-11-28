using System.Collections.Generic;
using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

/// <summary>
/// Applies month-based percentage multipliers to the subtotal (base + distance).
/// </summary>
public class SeasonalAdjustmentRule : IPriceRule
{
    /// <summary>
    /// Month number to seasonal uplift multiplier (e.g., 0.15 = +15% of subtotal).
    /// Extend or adjust entries to reflect promotional or peak-period pricing.
    /// </summary>
    public static readonly IReadOnlyDictionary<int, decimal> MonthlyMultipliers = new Dictionary<int, decimal>
    {
        [12] = 0.15m,
        [11] = 0.08m,
        [6] = 0.05m,
        [7] = 0.05m
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
