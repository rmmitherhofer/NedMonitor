using Microsoft.AspNetCore.Builder;

namespace NedMonitor.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
internal static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the specified middleware to the application's request pipeline only if it hasn't been added yet.
    /// </summary>
    /// <typeparam name="TMiddleware">The middleware type to add.</typeparam>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="app"/> is null.</exception>
    internal static IApplicationBuilder TryUseMiddleware<TMiddleware>(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        var key = typeof(TMiddleware).FullName!;
        if (!app.Properties.ContainsKey(key))
        {
            app.UseMiddleware<TMiddleware>();
            app.Properties[key] = true;
        }
        return app;
    }
}
