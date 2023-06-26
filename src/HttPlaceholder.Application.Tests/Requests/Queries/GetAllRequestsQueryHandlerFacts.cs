using HttPlaceholder.Application.Requests.Queries.GetAllRequests;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.Tests.Requests.Queries;

[TestClass]
public class GetAllRequestsQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_FromIdentifierAndItemsPerPageNotSet()
    {
        // Arrange
        var request = new GetAllRequestsQuery(null, null);
        var handler = _mocker.CreateInstance<GetAllRequestsQueryHandler>();

        var response = Array.Empty<RequestResultModel>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        stubContextMock
            .Setup(m => m.GetRequestResultsAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(response, result);
    }

    [TestMethod]
    public async Task Handle_FromIdentifierSet()
    {
        // Arrange
        var request = new GetAllRequestsQuery("abc123", null);
        var handler = _mocker.CreateInstance<GetAllRequestsQueryHandler>();

        var response = Array.Empty<RequestResultModel>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        stubContextMock
            .Setup(m => m.GetRequestResultsAsync(It.Is<PagingModel>(p => p.FromIdentifier == "abc123" && !p.ItemsPerPage.HasValue), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(response, result);
    }

    [TestMethod]
    public async Task Handle_FromIdentifierAndItemsPerPageSet()
    {
        // Arrange
        var request = new GetAllRequestsQuery("abc123", 2);
        var handler = _mocker.CreateInstance<GetAllRequestsQueryHandler>();

        var response = Array.Empty<RequestResultModel>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        stubContextMock
            .Setup(m => m.GetRequestResultsAsync(It.Is<PagingModel>(p => p.FromIdentifier == "abc123" && p.ItemsPerPage == 2), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(response, result);
    }
}
