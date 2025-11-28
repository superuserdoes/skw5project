using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing.Rules;

public class DistanceSurchargeRule : IPriceRule
{
    public void Apply(DeliveryOrder order, PriceCalculationContext context)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(context);

        var originValue = ExtractNumericPrefix(order.OriginPostalCode);
        var destinationValue = ExtractNumericPrefix(order.DestinationPostalCode);
        var distanceKm = Math.Abs(originValue - destinationValue) * 10m;

        if (distanceKm <= 0)
        {
            return;
        }

        var surchargeSteps = Math.Ceiling(distanceKm / 50m);
        var surcharge = surchargeSteps * 2.5m;
        context.AddDistanceSurcharge(surcharge);
    }

    private static int ExtractNumericPrefix(string code)
    {
        var digits = new string(code.Where(char.IsDigit).Take(5).ToArray());
        return int.TryParse(digits, out var value) ? value : 0;
    }
}
