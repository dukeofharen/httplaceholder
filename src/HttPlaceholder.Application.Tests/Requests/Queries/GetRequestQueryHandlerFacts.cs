using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Requests.Queries;
using HttPlaceholder.Application.StubExecution;

namespace HttPlaceholder.Application.Tests.Requests.Queries;

[TestClass]
public class GetRequestQueryHandlerFacts
{
    private readonly Mock<IStubContext> _mockStubContext = new();
    private GetRequestQueryHandler _handler;

    [TestInitialize]
    public void Initialize() => _handler = new GetRequestQueryHandler(_mockStubContext.Object);

    [TestCleanup]
    public void Cleanup() => _mockStubContext.VerifyAll();

    [TestMethod]
    public async Task Handle_RequestNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var query = new GetRequestQuery(correlationId);

        _mockStubContext
            .Setup(m => m.GetRequestResultAsync(correlationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RequestResultModel)null);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_RequestFound_ShouldReturnRequest()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var query = new GetRequestQuery(correlationId);

        var expectedResult = new RequestResultModel { CorrelationId = correlationId };
        _mockStubContext
            .Setup(m => m.GetRequestResultAsync(correlationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
