using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedService.Core.Handlers;

public static class HandlerExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes =>
                    classes.AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes =>
                    classes.AssignableToAny(typeof(IQueryHandler<,>), typeof(IQueryHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

        return services;
    }
}