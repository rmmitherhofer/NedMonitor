using Common.Extensions;
using Common.Http.Configurations;
using Common.Notifications.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tracezilla.Applications;
using Tracezilla.BackgroundServices;
using Tracezilla.Builders;
using Tracezilla.Configurations.Settings;
using Tracezilla.Enums;
using Tracezilla.Extensions;
using Tracezilla.Middleware;
using Tracezilla.Providers;
using Tracezilla.Queues;

namespace Tracezilla.Configurations;

/// <summary>
/// Extension methods for registering and configuring Tracezilla services and middleware.
/// </summary>
public static class TracezillaConfigurations
{
    /// <summary>
    /// Adds Tracezilla services to the specified <see cref="IServiceCollection"/> and configures it.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Application configuration to bind Tracezilla settings.</param>
    /// <param name="configure">Action to configure <see cref="FormatterOptions"/>.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddTracezilla(this IServiceCollection services, IConfiguration configuration, Action<FormatterOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddNotificationConfig();

        services.AddHttpConfig();

        services.AddOptions(configuration);

        services.AddService(configuration, configure);

        return services;
    }

    /// <summary>
    /// Registers Tracezilla configuration options from the provided configuration.
    /// </summary>
    /// <param name="services">The service collection to add options to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.Configure<TracezillaSettings>(configuration.GetSection(TracezillaSettings.TRACEZILLA_NODE));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TracezillaSettings>>().Value.SensitiveDataMasker;

            List<string> defaultKeys = ["password",    "senha",    "token",    "access_token",    "refresh_token",
                "jwt",    "jwe",    "jws",    "jwk",    "jwa",    "jwm",
                "auth",    "authentication",    "authorization",    "autenticacao",    "autorizacao",
                "secret",    "client_secret",    "api_key",    "secret_key",    "private_key",
                "assinatura",    "signature",    "segredo",    "pin",    "otp",    "mfa_code",    "codigo_mfa"
                ];

            var mergedKeys = defaultKeys
                .Concat(options.SensitiveKeys ?? Enumerable.Empty<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var finalOptions = new SensitiveDataMaskerOptions
            {
                SensitiveKeys = mergedKeys
            };

            return new SensitiveDataMasker(finalOptions);
        });

        return services;
    }

    /// <summary>
    /// Registers Tracezilla application services and logger provider.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="configure">Action to configure <see cref="FormatterOptions"/>.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration, Action<FormatterOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        FormatterOptions options = new();

        configure?.Invoke(options);

        var provider = services.BuildServiceProvider();
        var settings = provider.GetRequiredService<IOptions<TracezillaSettings>>().Value;

        if (settings.ExecutionMode == ExecutionMode.Disabled) return services;

        if (settings.ExecutionMode == ExecutionMode.Full)
        {
            services.AddSingleton<ILoggerProvider, LoggerProvider>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                return new LoggerProvider(options, httpContextAccessor);
            });
        }

        services.TryAddScoped<ITracezillaApplication, TracezillaApplication>();
        services.TryAddScoped<ILogContextBuilder, LogContextBuilder>();

        services.AddSingleton<ITracezillaQueue, TracezillaQueue>();
        services.AddHostedService<TracezillaBackgroundService>();

        services.AddHttpService(configuration);

        return services;
    }

    /// <summary>
    /// Adds Tracezilla middleware components into the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTracezilla(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.UseHttpConfig();

        var settings = app.ApplicationServices.GetRequiredService<IOptions<TracezillaSettings>>().Value;

        if (settings.ExecutionMode == ExecutionMode.Disabled) return app;

        app.TryUseMiddleware<TracezillaMiddleware>();

        app.TryUseMiddleware<BodyBufferingMiddleware>();

        app.TryUseMiddleware<CaptureResponseBodyMiddleware>();

        return app;
    }

}
