using System.Diagnostics.CodeAnalysis;
using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing;

public record PriceBreakdown(decimal BasePrice, decimal DistanceSurcharge, decimal SeasonalAdjustment, decimal TotalPrice)
{
    public static PriceBreakdown FromContext(PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var total = context.BasePrice + context.DistanceSurcharge + context.SeasonalAdjustment;
        return new PriceBreakdown(context.BasePrice, context.DistanceSurcharge, context.SeasonalAdjustment, total);
    }
}

public class PriceCalculationContext
{
    public decimal BasePrice { get; private set; }
    public decimal DistanceSurcharge { get; private set; }
    public decimal SeasonalAdjustment { get; private set; }

    public void SetBasePrice(decimal amount)
    {
        BasePrice = Normalize(amount);
    }

    public void AddDistanceSurcharge(decimal amount)
    {
        DistanceSurcharge += Normalize(amount);
    }

    public void AddSeasonalAdjustment(decimal amount)
    {
        SeasonalAdjustment += Normalize(amount);
    }

    [return: NotNull]
    private static decimal Normalize(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}

public interface IPricingService
{
    PriceBreakdown Calculate(DeliveryOrder order);
}
