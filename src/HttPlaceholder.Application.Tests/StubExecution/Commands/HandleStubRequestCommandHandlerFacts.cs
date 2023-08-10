using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Commands;

namespace HttPlaceholder.Application.Tests.StubExecution.Commands;

[TestClass]
public class HandleStubRequestCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();
        var handler = _mocker.CreateInstance<HandleStubRequestCommandHandler>();

        var expectedResponse = new ResponseModel();
        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await handler.Handle(new HandleStubRequestCommand(), CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }
}
