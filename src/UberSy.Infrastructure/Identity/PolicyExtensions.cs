using Microsoft.Extensions.DependencyInjection;
using UberSy.Domain.Identity;

namespace UberSy.Infrastructure.Identity;

public static class PolicyExtensions
{
    public static IServiceCollection AddCustomPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.AdminOnly, p => p.RequireRole( Roles.Admin))
            .AddPolicy(Policies.OwnerOnly, p => p.RequireRole(Roles.Owner))
            .AddPolicy(Policies.CustomerOnly, p => p.RequireRole(Roles.Customer))
            .AddPolicy(Policies.DriverOnly, p => p.RequireRole(Roles.Driver))
            .AddPolicy(Policies.AgentOnly, p => p.RequireRole(Roles.Agent));
        return services;
    }
}