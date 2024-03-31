using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.ScheduledJobs;
using HttPlaceholder.Application.ScheduledJobs.Queries;

namespace HttPlaceholder.Application.Tests.ScheduledJobs.Queries;

[TestClass]
public class GetScheduledJobNamesQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Setup() =>
        _mocker.Use<IEnumerable<ICustomHostedService>>(new[]
        {
            CreateCustomHostedService("job1"), CreateCustomHostedService("job2")
        });

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var handler = _mocker.CreateInstance<GetScheduledJobNamesQueryHandler>();

        // Act
        var result = (await handler.Handle(new GetScheduledJobNamesQuery(), CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("job1", result[0]);
        Assert.AreEqual("job2", result[1]);
    }

    private static ICustomHostedService CreateCustomHostedService(string key)
    {
        var mock = new Mock<ICustomHostedService>();
        mock
            .Setup(m => m.Key)
            .Returns(key);
        return mock.Object;
    }
}
