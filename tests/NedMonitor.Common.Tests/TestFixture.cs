using AutoFixture;
using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NSubstitute;
using Zypher.Notifications.Interfaces;

namespace NedMonitor.Common.Tests;

public abstract class TestFixture<T>
{
    public Fixture Auto { get; protected set; }
    public Faker Faker { get; protected set; }
    public AutoMocker Mocker { get; protected set; }
    public IServiceCollection Services { get; protected set; }
    public IConfiguration Configuration { get; protected set; }
    public IWebHostEnvironment Environment { get; protected set; }
    public Mock<ILogger<T>> Logger { get; protected set; }
    public Mock<INotificationHandler> Notification { get; protected set; }
    protected TestFixture()
    {
        Services = new ServiceCollection();

        var currentDirectory = Directory.GetCurrentDirectory().Replace("\\", "/");

        string? commonDirectory = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.FullName.Contains("Common.Tests"))
            .Select(x => x.Location)
            .FirstOrDefault()
            .Replace("\\", "/");

        int lastBackslashIndex = commonDirectory.LastIndexOf('/');
        int lastDotIndex = commonDirectory.LastIndexOf('.');

        commonDirectory = commonDirectory.Substring(lastBackslashIndex + 1, lastDotIndex - lastBackslashIndex - 1);
        currentDirectory = currentDirectory.Substring(0, currentDirectory.IndexOf("tests"));

        var contentRoot = currentDirectory + "tests/" + commonDirectory + "/Properties";

        Configuration = new ConfigurationBuilder()
            .SetBasePath(contentRoot)
            .AddJsonFile("appsettings.testing.json")
            .Build();

        Environment = Substitute.For<IWebHostEnvironment>();
    }

    protected void InitializeAutoFixture()
    {
        Auto = new();
        Faker = new();
    }
}