using HttPlaceholder.Application.Requests.Commands;
using HttPlaceholder.Application.StubExecution;

namespace HttPlaceholder.Application.Tests.Requests.Commands;

[TestClass]
public class DeleteRequestCommandHandlerFacts
{
    private readonly Mock<IStubContext> _mockStubContext = new();
    private DeleteRequestCommandHandler _handler;

    [TestInitialize]
    public void Initialize() => _handler = new DeleteRequestCommandHandler(_mockStubContext.Object);

    [TestCleanup]
    public void Cleanup() => _mockStubContext.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var request = new DeleteRequestCommand(Guid.NewGuid().ToString());
        _mockStubContext
            .Setup(m => m.DeleteRequestAsync(request.CorrelationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        _mockStubContext.Verify(m => m.DeleteRequestAsync(request.CorrelationId, It.IsAny<CancellationToken>()));
    }
}
