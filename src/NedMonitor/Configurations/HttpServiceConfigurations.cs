using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NedMonitor.Configurations.Settings;
using NedMonitor.HttpServices;

namespace NedMonitor.Configurations;

internal static class HttpServiceConfigurations
{
    /// <summary>
    /// Registers HTTP client services for NedMonitor integration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Application configuration instance.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        var settings = configuration.GetSection(NedMonitorSettings.NEDMONITOR_NODE).Get<NedMonitorSettings>() ?? new();

        services.AddHttpClient<INedMonitorHttpService, NedMonitorHttpService>(client =>
            client.BaseAddress = new Uri(settings.Service.BaseAddress)
        );

        return services;
    }
}