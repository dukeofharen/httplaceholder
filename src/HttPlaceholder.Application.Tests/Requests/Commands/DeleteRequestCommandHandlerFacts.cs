using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Requests.Commands.DeleteRequest;
using HttPlaceholder.Application.StubExecution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.Requests.Commands
{
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

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockStubContext.Verify(m => m.DeleteRequestAsync(request.CorrelationId));
;        }
    }
}
