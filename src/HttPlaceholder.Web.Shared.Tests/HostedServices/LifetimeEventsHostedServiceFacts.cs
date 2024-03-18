using HttPlaceholder.Web.Shared.HostedServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Web.Shared.Tests.HostedServices;

[TestClass]
public class LifetimeEventsHostedServiceFacts
{
    private readonly MockLogger<LifetimeEventsHostedService> _logger = new();
    private readonly AutoMocker _mocker = new();

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
        await source.CancelAsync();

        // Assert
        var entry = _logger.Entries.Single(e => e.LogLevel == LogLevel.Information);
        Assert.IsTrue(entry.State.Contains("Starting the application took"));
    }
}
