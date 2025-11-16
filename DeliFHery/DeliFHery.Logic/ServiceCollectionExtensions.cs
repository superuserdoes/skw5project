using DeliFHery.Domain;
using DeliFHery.Logic;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDeliFHeryLogic(this IServiceCollection services)
    {
        services.AddScoped<IEntityValidator<Customer>, CustomerValidator>();
        services.AddScoped<IEntityValidator<DeliveryOrder>, DeliveryOrderValidator>();
        services.AddScoped<IEntityValidator<Contact>, ContactValidator>();
        services.AddScoped<IDeliFHeryLogic, DeliFHeryLogic>();
        return services;
    }
}
