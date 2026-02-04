using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Applications;
using NedMonitor.BackgroundServices;
using NedMonitor.Builders;
using NedMonitor.Core;
using NedMonitor.Core.Extensions;
using NedMonitor.Core.Settings;
using NedMonitor.Middleware;
using NedMonitor.Providers;
using NedMonitor.Queues;

namespace NedMonitor.Configurations;

/// <summary>
/// Extension methods for registering and configuring NedMonitor services and middleware.
/// </summary>
public static class NedMonitorConfigurations
{
    /// <summary>
    /// Adds NedMonitor services to the specified <see cref="IServiceCollection"/> and configures it.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Application configuration to bind NedMonitor settings.</param>
    /// <param name="configure">Action to configure <see cref="FormatterOptions"/>.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddNedMonitor(this IServiceCollection services, IConfiguration configuration, Action<FormatterOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        if(!configuration.NedMonitorIsEnabled()) return services;

        services.AddHttpContextAccessor();

        services.AddOptions(configuration);

        services.AddService(configuration, configure);

        return services;
    }

    /// <summary>
    /// Registers NedMonitor configuration options from the provided configuration.
    /// </summary>
    /// <param name="services">The service collection to add options to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services
            .AddOptions<NedMonitorSettings>()
            .Bind(configuration.GetSection(NedMonitorSettings.NEDMONITOR_NODE))
            .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<NedMonitorSettings>, NedMonitorSettingsValidation>();

        services.PostConfigure<NedMonitorSettings>(settings =>
        {
            bool dbQueryEnabled = settings.ExecutionMode.EnableMonitorDbQueries;

            if (settings.DataInterceptors?.EF is not null &&
                (settings.DataInterceptors.EF.Enabled == default || !dbQueryEnabled))
            {
                settings.DataInterceptors.EF.Enabled = dbQueryEnabled;
            }

            if (settings.DataInterceptors?.Dapper is not null &&
                (settings.DataInterceptors.Dapper.Enabled == default || !dbQueryEnabled))
            {
                settings.DataInterceptors.Dapper.Enabled = dbQueryEnabled;
            }

            settings.SensitiveDataMasking.SensitiveKeys = NedMonitorConstants.DEFAULT_KEYS
               .Concat(settings.SensitiveDataMasking?.SensitiveKeys ?? Enumerable.Empty<string>())
               .Distinct(StringComparer.OrdinalIgnoreCase)
               .ToList();
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<NedMonitorSettings>>().Value.SensitiveDataMasking;

            return new SensitiveDataMasker(options);
        });

        return services;
    }

    /// <summary>
    /// Registers NedMonitor application services and logger provider.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="configure">Action to configure <see cref="FormatterOptions"/>.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration, Action<FormatterOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        FormatterOptions options = new();

        configure?.Invoke(options);

        var monitorSection = configuration.GetSection(NedMonitorSettings.NEDMONITOR_NODE);
        var settings = monitorSection.Get<NedMonitorSettings>();

        if (settings?.ExecutionMode.EnableNedMonitor != true) return services;

        if (settings.ExecutionMode.EnableMonitorLogs)
        {
            services.AddSingleton<ILoggerProvider, LoggerProvider>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var configOptions = sp.GetRequiredService<IOptions<NedMonitorSettings>>();
                return new LoggerProvider(options, httpContextAccessor, configOptions);
            });
        }

        services.TryAddScoped<INedMonitorApplication, NedMonitorApplication>();
        services.TryAddScoped<ILogContextBuilder, LogContextBuilder>();

        services.TryAddSingleton<INedMonitorQueue, NedMonitorQueue>();
        services.AddHostedService<NedMonitorBackgroundService>();

        services.AddHttpService(configuration);

        return services;
    }

    /// <summary>
    /// Adds NedMonitor middleware components into the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseNedMonitor(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        if (!app.NedMonitorIsEnabled()) return app;

        var settings = app.ApplicationServices.GetRequiredService<IOptions<NedMonitorSettings>>().Value;

        if (!settings.ExecutionMode.EnableNedMonitor) return app;

        app.TryUseMiddleware<NedMonitorMiddleware>();

        app.TryUseMiddleware<BodyBufferingMiddleware>();

        app.TryUseMiddleware<CaptureResponseBodyMiddleware>();

        return app;
    }

    /// <summary>
    /// Adds the NedMonitor exception capture middleware to the application's request pipeline,
    /// if the execution mode is configured to monitor exceptions.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    public static IApplicationBuilder UseNedMonitorMiddleware(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        if (!app.NedMonitorIsEnabled()) return app;

        var settings = app.ApplicationServices.GetRequiredService<IOptions<NedMonitorSettings>>().Value;

        if (!settings.ExecutionMode.EnableNedMonitor || !settings.ExecutionMode.EnableMonitorExceptions) return app;

        app.TryUseMiddleware<NedMonitorExceptionCaptureMiddleware>();

        return app;
    }

    private static bool NedMonitorIsEnabled(this IApplicationBuilder app)
    {
        var monitorSection = app.ApplicationServices
            .GetRequiredService<IConfiguration>()
            .GetSection(NedMonitorSettings.NEDMONITOR_NODE);

        var settingsFromSection = monitorSection.Get<NedMonitorSettings>();

        return settingsFromSection?.ExecutionMode?.EnableNedMonitor == true;
    }

    private static bool NedMonitorIsEnabled(this IConfiguration configuration)
    {
        var monitorSection = configuration.GetSection(NedMonitorSettings.NEDMONITOR_NODE);
        var settings = monitorSection.Get<NedMonitorSettings>();

        return settings?.ExecutionMode.EnableNedMonitor == true;
    }
}
