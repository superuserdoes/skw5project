using DeliFHery.Domain;

namespace DeliFHery.Logic.Pricing;

/// <summary>
/// Orchestrates price calculation by running all registered <see cref="IPriceRule"/> instances
/// sequentially. The injected order of the rules controls the execution sequence, allowing
/// composition without changing this coordinator.
/// </summary>
public class PricingService(IEnumerable<IPriceRule> rules) : IPricingService
{   
    private readonly IReadOnlyCollection<IPriceRule> _rules = rules?.ToList()
        ?? throw new ArgumentNullException(nameof(rules));

    /// <summary>
    /// Calculates a <see cref="PriceBreakdown"/> for the given <paramref name="order"/> by applying all
    /// known pricing rules. The same pipeline is used by the API for quotes and during delivery creation.
    /// </summary>
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
