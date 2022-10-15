using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.Stubs.Queries.GetStubsOverview;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Stubs.Queries;

[TestClass]
public class GetStubsOverviewQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var handler = _mocker.CreateInstance<GetStubsOverviewQueryHandler>();
        var mockStubContext = _mocker.GetMock<IStubContext>();

        var stubs = Array.Empty<FullStubOverviewModel>();
        mockStubContext
            .Setup(m => m.GetStubsOverviewAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        // Act
        var result = await handler.Handle(new GetStubsOverviewQuery(), CancellationToken.None);

        // Assert
        Assert.AreEqual(stubs, result);
    }
}
