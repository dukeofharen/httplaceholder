using System.Linq;
using HttPlaceholder.HostedServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Tests.HostedServices;

[TestClass]
public class LifetimeEventsHostedServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockLogger<LifetimeEventsHostedService> _logger = new();

    [TestInitialize]
    public void Initialize() => _mocker.Use<ILogger<LifetimeEventsHostedService>>(_logger);

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task StartAsync_HappyFlow()
    {
        // Arrange
        var lifetimeMock = _mocker.GetMock<IHostApplicationLifetime>();
        var service = _mocker.CreateInstance<LifetimeEventsHostedService>();

        var source = new CancellationTokenSource();
        var token = source.Token;
        lifetimeMock
            .Setup(m => m.ApplicationStarted)
            .Returns(token);

        // Act
        await service.StartAsync(CancellationToken.None);
        source.Cancel();

        // Assert
        var entry = _logger.Entries.Single(e => e.LogLevel == LogLevel.Information);
        Assert.IsTrue(entry.State.Contains("Starting the application took"));
    }
}
