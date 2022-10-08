using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.Stubs.Queries.GetStub;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Stubs.Queries;

[TestClass]
public class GetStubQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_StubNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<GetStubQueryHandler>();
        var mockStubContext = _mocker.GetMock<IStubContext>();

        var request = new GetStubQuery("stub1");

        mockStubContext
            .Setup(m => m.GetStubAsync(request.StubId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FullStubModel)null);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(request, CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var handler = _mocker.CreateInstance<GetStubQueryHandler>();
        var mockStubContext = _mocker.GetMock<IStubContext>();

        var request = new GetStubQuery("stub1");

        var stub = new FullStubModel();
        mockStubContext
            .Setup(m => m.GetStubAsync(request.StubId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(stub, result);
    }
}
