using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Interfaces;
using NedMonitor.Core.Settings;
using NedMonitor.EF.Configurations;
using NedMonitor.EF.Interceptors;
using Xunit.Abstractions;

namespace NedMonitor.EF.Tests.Configurations;

public class EfInterceptorsConfigurationTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given null services, " +
        "When adding EF interceptors, " +
        "Then it throws")]
    [Trait("Configurations", nameof(EfInterceptorsConfiguration))]
    public async Task AddNedMonitorEfInterceptors_NullServices_Throws()
    {
        //Given
        IServiceCollection services = null!;

        //When
        var act = () => services.AddNedMonitorEfInterceptors();

        //Then
        act.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given services, " +
        "When adding EF interceptors twice, " +
        "Then it registers single instances")]
    [Trait("Configurations", nameof(EfInterceptorsConfiguration))]
    public async Task AddNedMonitorEfInterceptors_Twice_RegistersOnce()
    {
        //Given
        var services = new ServiceCollection();

        //When
        services.AddNedMonitorEfInterceptors();
        services.AddNedMonitorEfInterceptors();

        //Then
        services.Count(s => s.ServiceType == typeof(IQueryCounter)).Should().Be(1);
        services.Count(s => s.ServiceType == typeof(EfQueryCounter)).Should().Be(1);
        services.Count(s => s.ServiceType == typeof(IHttpContextAccessor)).Should().Be(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given custom registrations, " +
        "When adding EF interceptors, " +
        "Then it does not override them")]
    [Trait("Configurations", nameof(EfInterceptorsConfiguration))]
    public async Task AddNedMonitorEfInterceptors_CustomRegistrations_NotOverridden()
    {
        //Given
        var services = new ServiceCollection();
        services.AddScoped<IQueryCounter, CustomQueryCounter>();
        services.AddSingleton<EfQueryCounter>(_ => new EfQueryCounter(
            new CustomQueryCounter(),
            new HttpContextAccessor(),
            Options.Create(new NedMonitorSettings())));

        //When
        services.AddNedMonitorEfInterceptors();

        //Then
        services.Count(s => s.ServiceType == typeof(IQueryCounter)).Should().Be(1);
        services.Count(s => s.ServiceType == typeof(EfQueryCounter)).Should().Be(1);
        services.Single(s => s.ServiceType == typeof(IQueryCounter)).ImplementationType.Should().Be(typeof(CustomQueryCounter));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given null app, " +
        "When using EF interceptors, " +
        "Then it throws")]
    [Trait("Configurations", nameof(EfInterceptorsConfiguration))]
    public async Task UseNedMonitorEfInterceptors_NullApp_Throws()
    {
        //Given
        IApplicationBuilder app = null!;

        //When
        var act = () => app.UseNedMonitorEfInterceptors();

        //Then
        act.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    private sealed class CustomQueryCounter : IQueryCounter
    {
        public int GetCount(HttpContext context) => 0;
        public void Increment() { }
        public void Reset(HttpContext context) { }
    }
}
