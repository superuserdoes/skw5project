using DeliFHery.Domain;
using DeliFHery.Logic;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDeliFHeryLogic(this IServiceCollection services)
    {
        services.AddSingleton<IEntityValidator<Customer>, CustomerValidator>();
        services.AddSingleton<IEntityValidator<DeliveryOrder>, DeliveryOrderValidator>();
        services.AddSingleton<IEntityValidator<Contact>, ContactValidator>();
        services.AddSingleton<IDeliFHeryLogic, DeliFHeryLogic>();
        return services;
    }
}
