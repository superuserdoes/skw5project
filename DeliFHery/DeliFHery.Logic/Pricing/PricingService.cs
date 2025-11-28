using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing;

public class PricingService(IEnumerable<IPriceRule> rules) : IPricingService
{
    private readonly IReadOnlyCollection<IPriceRule> _rules = rules?.ToList()
        ?? throw new ArgumentNullException(nameof(rules));

    public PriceBreakdown Calculate(DeliveryOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var context = new PriceCalculationContext();
        foreach (var rule in _rules)
        {
            rule.Apply(order, context);
        }

        return PriceBreakdown.FromContext(context);
    }
}
