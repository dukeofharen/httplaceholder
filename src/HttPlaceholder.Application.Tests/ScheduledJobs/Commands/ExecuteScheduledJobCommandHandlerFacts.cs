using System.Collections.Generic;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.ScheduledJobs;
using HttPlaceholder.Application.ScheduledJobs.Commands.ExecuteScheduledJob;

namespace HttPlaceholder.Application.Tests.ScheduledJobs.Commands;

[TestClass]
public class ExecuteScheduledJobCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly Mock<ICustomHostedService> _mockJob1 = CreateCustomHostedServiceMock("job1");
    private readonly Mock<ICustomHostedService> _mockJob2 = CreateCustomHostedServiceMock("job2");

    [TestInitialize]
    public void Setup() => _mocker.Use<IEnumerable<ICustomHostedService>>(new[] { _mockJob1.Object, _mockJob2.Object });

    [TestMethod]
    public async Task Handle_JobNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ExecuteScheduledJobCommandHandler>();
        var request = new ExecuteScheduledJobCommand("job3");

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual("Hosted service with key 'job3'.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_ExceptionWhenRunningScheduledJob()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ExecuteScheduledJobCommandHandler>();
        var request = new ExecuteScheduledJobCommand("job1");
        _mockJob1
            .Setup(m => m.ProcessAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception("ERROR!"));

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Failed);
        Assert.IsTrue(result.Message.Contains("ERROR!"));
    }

    [TestMethod]
    public async Task Handle_JobExecutesSuccessfully()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ExecuteScheduledJobCommandHandler>();
        var request = new ExecuteScheduledJobCommand("job2");


        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Failed);
        Assert.AreEqual("OK", result.Message);
        _mockJob2.Verify(m => m.ProcessAsync(It.IsAny<CancellationToken>()));
    }

    private static Mock<ICustomHostedService> CreateCustomHostedServiceMock(string key)
    {
        var mock = new Mock<ICustomHostedService>();
        mock
            .Setup(m => m.Key)
            .Returns(key);
        return mock;
    }
}
