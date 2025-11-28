using DeliFHery.Domain;
using DeliFHery.Logic;
using DeliFHery.Logic.Pricing;
using DeliFHery.Logic.Pricing.Rules;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDeliFHeryLogic(this IServiceCollection services)
    {
        services.AddScoped<IEntityValidator<Customer>, CustomerValidator>();
        services.AddScoped<IEntityValidator<DeliveryOrder>, DeliveryOrderValidator>();
        services.AddScoped<IEntityValidator<Contact>, ContactValidator>();
        services.AddSingleton<IPriceRule, WeightPriceRule>();
        services.AddSingleton<IPriceRule, DistanceSurchargeRule>();
        services.AddSingleton<IPriceRule, SeasonalAdjustmentRule>();
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<IDeliFHeryLogic, DeliFHeryLogic>();
        return services;
    }
}
