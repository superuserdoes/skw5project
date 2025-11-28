using System.Diagnostics.CodeAnalysis;
using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing;

/// <summary>
/// Exposes the final pricing values surfaced via API and persisted with deliveries.
/// </summary>
public record PriceBreakdown(decimal BasePrice, decimal DistanceSurcharge, decimal SeasonalAdjustment, decimal TotalPrice)
{
    /// <summary>
    /// Creates a <see cref="PriceBreakdown"/> from the mutable <see cref="PriceCalculationContext"/>,
    /// aggregating the currently tracked totals.
    /// </summary>
    public static PriceBreakdown FromContext(PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var total = context.BasePrice + context.DistanceSurcharge + context.SeasonalAdjustment;
        return new PriceBreakdown(context.BasePrice, context.DistanceSurcharge, context.SeasonalAdjustment, total);
    }
}

/// <summary>
/// Shared state passed through each pricing rule. Rules mutate the tracked components in a controlled,
/// rounded manner to ensure consistent totals.
/// </summary>
public class PriceCalculationContext
{
    public decimal BasePrice { get; private set; }
    public decimal DistanceSurcharge { get; private set; }
    public decimal SeasonalAdjustment { get; private set; }

    /// <summary>Sets the base price, overwriting any previous value.</summary>
    public void SetBasePrice(decimal amount)
    {
        BasePrice = Normalize(amount);
    }

    /// <summary>Adds a distance surcharge to the running total.</summary>
    public void AddDistanceSurcharge(decimal amount)
    {
        DistanceSurcharge += Normalize(amount);
    }

    /// <summary>Adds a seasonal adjustment to the running total.</summary>
    public void AddSeasonalAdjustment(decimal amount)
    {
        SeasonalAdjustment += Normalize(amount);
    }

    [return: NotNull]
    private static decimal Normalize(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}

/// <summary>
/// Service abstraction for pricing calculations to support DI and testing.
/// </summary>
public interface IPricingService
{
    PriceBreakdown Calculate(DeliveryOrder order);
}
