using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UberSy.Application.Common.Behaviours;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Services;
using UberSy.Application.Features.ServiceRequests.Services;

namespace UberSy.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
        });

        services.AddScoped<IDriverDispatchService, DriverDispatchService>();
        services.AddScoped<IDeliveryPricingService, DeliveryPricingService>();
        services.AddScoped<IFoodOrderCalculator, FoodOrderCalculator>();

        return services;
    }
}
