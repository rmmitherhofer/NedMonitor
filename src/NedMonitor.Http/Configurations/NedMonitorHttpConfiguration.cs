using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NedMonitor.Http.Handlers;

namespace NedMonitor.Http.Configurations;

public static class NedMonitorHttpConfiguration
{
    public static IServiceCollection AddNedMonitorHttp(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.TryAddTransient<NedMonitorHttpLoggingHandler>();

        return services;
    }
}
