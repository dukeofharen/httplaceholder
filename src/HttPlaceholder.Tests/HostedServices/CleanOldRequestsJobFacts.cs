using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.HostedServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Tests.HostedServices;

[TestClass]
public class CleanOldRequestsJobFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ProcessAsync_HappyFlow()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var job = _mocker.CreateInstance<CleanOldRequestsJob>();

        // Act
        await job.ProcessAsync(CancellationToken.None);

        // Assert
        stubContextMock.Verify(m => m.CleanOldRequestResultsAsync(It.IsAny<CancellationToken>()));
    }
}
