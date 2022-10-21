using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using HttPlaceholder.HostedServices;
using HttPlaceholder.TestUtilities.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Tests.HostedServices;

[TestClass]
public class BackgroundServiceFacts
{
    private readonly MockLogger<BackgroundService> _loggerMock = new();
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize() => _mocker.Use<ILogger<BackgroundService>>(_loggerMock);

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task StartAsync_NotTimeYet_ShouldNotExecuteService()
    {
        // Arrange
        var dateTimeMock = _mocker.GetMock<IDateTime>();
        var now = new DateTime(2022, 2, 6, 4, 4, 59);
        dateTimeMock
            .Setup(m => m.Now)
            .Returns(now);

        var service = _mocker.CreateInstance<SucceedingBackgroundService>();

        service.StoppingCts.Cancel();

        // Act
        await service.StartAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(service.ProcessCalled);
    }

    [TestMethod]
    public async Task StartAsync_ItsTime_ShouldExecuteService()
    {
        // Arrange
        var dateTimeMock = _mocker.GetMock<IDateTime>();
        dateTimeMock
            .Setup(m => m.Now)
            .Returns(new DateTime(2022, 2, 6, 4, 4, 59));

        var service = _mocker.CreateInstance<SucceedingBackgroundService>();

        dateTimeMock
            .Setup(m => m.Now)
            .Returns(new DateTime(2022, 2, 6, 4, 5, 1));

        service.StoppingCts.Cancel();

        // Act
        await service.StartAsync(CancellationToken.None);

        // Assert
        Assert.IsTrue(service.ProcessCalled);
    }

    [TestMethod]
    public async Task StartAsync_ItsTime_ProcessingThrowsException_ShouldLogError()
    {
        // Arrange
        var dateTimeMock = _mocker.GetMock<IDateTime>();
        dateTimeMock
            .Setup(m => m.Now)
            .Returns(new DateTime(2022, 2, 6, 4, 4, 59));

        var service = _mocker.CreateInstance<FailingBackgroundService>();

        dateTimeMock
            .Setup(m => m.Now)
            .Returns(new DateTime(2022, 2, 6, 4, 5, 1));

        service.StoppingCts.Cancel();

        // Act
        await service.StartAsync(CancellationToken.None);

        // Assert
        Assert.IsTrue(service.ProcessCalled);
        var logEvent = _loggerMock.Entries.Single(e => e.LogLevel == LogLevel.Error);
        Assert.IsTrue(logEvent.State.Contains("Unexpected exception thrown while executing service") &&
                      logEvent.State.Contains("PROCESSING WENT WRONG!"));
    }

    [TestMethod]
    public async Task StopAsync_StopCalledWithoutStart_ShouldDoNothing()
    {
        // Arrange
        var service = _mocker.CreateInstance<SucceedingBackgroundService>();

        // Act
        await service.StopAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(service.StoppingCts.IsCancellationRequested);
    }

    [TestMethod]
    public async Task StopAsync_ShouldCancelService()
    {
        // Arrange
        var service = _mocker.CreateInstance<SucceedingBackgroundService>();
        service.ExecutingTask = Task.CompletedTask;

        // Act
        await service.StopAsync(CancellationToken.None);

        // Assert
        Assert.IsTrue(service.StoppingCts.IsCancellationRequested);
    }

    private class SucceedingBackgroundService : BackgroundService
    {
        public SucceedingBackgroundService(
            ILogger<BackgroundService> logger,
            IDateTime dateTime,
            IAsyncService asyncService) : base(logger, dateTime, asyncService)
        {
        }

        public bool ProcessCalled { get; set; }

        public override string Schedule => "5 4 * * *";

        public override string Key => "succeeding";

        public override string Description => "A succeeding background job.";

        public override Task ProcessAsync(CancellationToken cancellationToken)
        {
            ProcessCalled = true;
            return Task.CompletedTask;
        }
    }

    private class FailingBackgroundService : BackgroundService
    {
        public FailingBackgroundService(
            ILogger<BackgroundService> logger,
            IDateTime dateTime,
            IAsyncService asyncService) : base(logger, dateTime, asyncService)
        {
        }

        public bool ProcessCalled { get; set; }

        public override string Schedule => "5 4 * * *";

        public override string Key => "failing";

        public override string Description => "A failing background job.";

        public override Task ProcessAsync(CancellationToken cancellationToken)
        {
            ProcessCalled = true;
            throw new InvalidOperationException("PROCESSING WENT WRONG!");
        }
    }
}
