using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Interfaces;
using NedMonitor.Dapper.Wrappers;
using NedMonitor.DataInterceptors.Middleware;
using Zypher.Extensions.Core;

namespace NedMonitor.Dapper.Configurations;

public static class DapperInterceptorsConfiguration
{
    public static IServiceCollection AddNedMonitorDapperInterceptors(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        services.TryAddScoped<IQueryCounter, HttpContextQueryCounter>();
        services.TryAddScoped<INedDapperWrapper, NedDapperWrapper>();

        return services;
    }

    public static IApplicationBuilder UseNedMonitorDapperInterceptors(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.TryUseMiddleware<QueryCounterResetMiddleware>();

        return app;
    }
}