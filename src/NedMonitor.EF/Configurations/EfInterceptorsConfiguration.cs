using Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Interfaces;
using NedMonitor.DataInterceptors.Middleware;
using NedMonitor.EF.Interceptors;

namespace NedMonitor.EF.Configurations;

/// <summary>
/// Provides extension methods to register and configure Entity Framework Core interceptors
/// for query counting and logging via NedMonitor.
/// </summary>
public static class EfInterceptorsConfiguration
{
    /// <summary>
    /// Registers the required EF Core interceptors and services used by NedMonitor,
    /// including query counter tracking per request.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddNedMonitorEfInterceptors(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        services.TryAddScoped<IQueryCounter, HttpContextQueryCounter>();
        services.TryAddScoped<EfQueryCounter>();

        return services;
    }

    /// <summary>
    /// Adds the middleware required to reset query counters at the beginning of each HTTP request.
    /// </summary>
    /// <param name="app">The application builder to configure the middleware pipeline.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseNedMonitorEfInterceptors(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.TryUseMiddleware<QueryCounterResetMiddleware>();

        return app;
    }
}