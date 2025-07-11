using Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NedMonitor.DataInterceptors.Interceptors;
using NedMonitor.DataInterceptors.Services;

namespace NedMonitor.DataInterceptors.Configurations;

public static class NedMonitorDataInterceptorsConfigurations
{
    public static IServiceCollection AddNedMonitor(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.TryAddSingleton<EfQueryCounter>();
        services.TryAddSingleton<DapperQueryCounter>();
        services.TryAddSingleton<IQueryCounter, CompositeQueryCounter>();

        return services;
    }


    public static IApplicationBuilder UseNedMonitorMiddleware(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.TryUseMiddleware<QueryCounterResetMiddleware>();

        return app;
    }
}
