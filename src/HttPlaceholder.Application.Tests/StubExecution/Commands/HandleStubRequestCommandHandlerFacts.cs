using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Commands.HandleStubRequest;

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
        var handler = _mocker.CreateInstance<HandleStubRequestCommandHandler>();

        // Act
        await handler.Handle(new HandleStubRequestCommand(), CancellationToken.None);

        // Assert
        _mocker.GetMock<IStubHandler>().Verify(m => m.HandleStubRequestAsync(It.IsAny<CancellationToken>()));
    }
}
