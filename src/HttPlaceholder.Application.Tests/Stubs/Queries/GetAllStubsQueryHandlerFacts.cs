using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.Stubs.Queries.GetAllStubs;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Stubs.Queries;

[TestClass]
public class GetAllStubsQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var handler = _mocker.CreateInstance<GetAllStubsQueryHandler>();
        var mockStubContext = _mocker.GetMock<IStubContext>();

        var stubs = Array.Empty<FullStubModel>();
        mockStubContext
            .Setup(m => m.GetStubsAsync())
            .ReturnsAsync(stubs);

        // Act
        var result = await handler.Handle(new GetAllStubsQuery(), CancellationToken.None);

        // Assert
        Assert.AreEqual(stubs, result);
    }
}
