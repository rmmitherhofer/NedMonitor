using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Moq;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Interfaces;
using NedMonitor.Dapper.Configurations;
using NedMonitor.Dapper.Wrappers;
using Xunit.Abstractions;

namespace NedMonitor.Dapper.Tests.Configurations;

public class DapperInterceptorsConfigurationTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given null services, " +
        "When adding Dapper interceptors, " +
        "Then it throws")]
    [Trait("Configurations", nameof(DapperInterceptorsConfiguration))]
    public async Task AddNedMonitorDapperInterceptors_NullServices_Throws()
    {
        //Given
        IServiceCollection services = null!;

        //When
        var act = () => services.AddNedMonitorDapperInterceptors();

        //Then
        act.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given services, " +
        "When adding Dapper interceptors twice, " +
        "Then it registers single instances")]
    [Trait("Configurations", nameof(DapperInterceptorsConfiguration))]
    public async Task AddNedMonitorDapperInterceptors_Twice_RegistersOnce()
    {
        //Given
        var services = new ServiceCollection();

        //When
        services.AddNedMonitorDapperInterceptors();
        services.AddNedMonitorDapperInterceptors();

        //Then
        services.Count(s => s.ServiceType == typeof(IQueryCounter)).Should().Be(1);
        services.Count(s => s.ServiceType == typeof(INedDapperWrapper)).Should().Be(1);
        services.Count(s => s.ServiceType == typeof(IHttpContextAccessor)).Should().Be(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given custom registrations, " +
        "When adding Dapper interceptors, " +
        "Then it does not override them")]
    [Trait("Configurations", nameof(DapperInterceptorsConfiguration))]
    public async Task AddNedMonitorDapperInterceptors_CustomRegistrations_NotOverridden()
    {
        //Given
        var services = new ServiceCollection();
        var customWrapper = new Mock<INedDapperWrapper>().Object;
        services.AddScoped<IQueryCounter, CustomQueryCounter>();
        services.AddSingleton<INedDapperWrapper>(customWrapper);

        //When
        services.AddNedMonitorDapperInterceptors();

        //Then
        services.Count(s => s.ServiceType == typeof(IQueryCounter)).Should().Be(1);
        services.Count(s => s.ServiceType == typeof(INedDapperWrapper)).Should().Be(1);
        services.Single(s => s.ServiceType == typeof(IQueryCounter)).ImplementationType.Should().Be(typeof(CustomQueryCounter));
        services.Single(s => s.ServiceType == typeof(INedDapperWrapper)).ImplementationInstance.Should().BeSameAs(customWrapper);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given null app, " +
        "When using Dapper interceptors, " +
        "Then it throws")]
    [Trait("Configurations", nameof(DapperInterceptorsConfiguration))]
    public async Task UseNedMonitorDapperInterceptors_NullApp_Throws()
    {
        //Given
        IApplicationBuilder app = null!;

        //When
        var act = () => app.UseNedMonitorDapperInterceptors();

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
