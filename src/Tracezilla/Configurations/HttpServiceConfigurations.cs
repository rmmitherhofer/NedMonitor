using Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tracezilla.Configurations.Settings;
using Tracezilla.HttpServices;

namespace Tracezilla.Configurations;

internal static class HttpServiceConfigurations
{
    /// <summary>
    /// Registers HTTP client services for Tracezilla integration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Application configuration instance.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        var settings = configuration.GetSection(TracezillaSettings.TRACEZILLA_NODE).Get<TracezillaSettings>() ?? new();

        services.AddHttpClient<ITracezillaHttpService, TracezillaHttpService>(client =>
            client.BaseAddress = new Uri(settings.Service.BaseAddress)
        );

        return services;
    }
}