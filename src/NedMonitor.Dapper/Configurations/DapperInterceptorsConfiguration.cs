using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NedMonitor.DataInterceptors.Middleware;
using Zypher.Extensions.Core;

namespace NedMonitor.DataInterceptors.Configurations;

public static class DapperInterceptorsConfiguration
{
    public static IServiceCollection AddNedMonitorDapperInterceptors(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        return services;
    }

    public static IApplicationBuilder UseNedMonitorDapperInterceptors(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.TryUseMiddleware<QueryCounterResetMiddleware>();

        return app;
    }
}