using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Requests.Queries.GetRequest;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.Requests.Queries
{
    [TestClass]
    public class GetRequestQueryHandlerFacts
    {
        private readonly Mock<IStubContext> _mockStubContext = new Mock<IStubContext>();
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
            var query = new GetRequestQuery {CorrelationId = correlationId};

            _mockStubContext
                .Setup(m => m.GetRequestResultAsync(correlationId))
                .ReturnsAsync((RequestResultModel)null);

            // Act / Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_RequestFound_ShouldReturnRequest()
        {
            // Arrange
            var correlationId = Guid.NewGuid().ToString();
            var query = new GetRequestQuery {CorrelationId = correlationId};

            var expectedResult = new RequestResultModel {CorrelationId = correlationId};
            _mockStubContext
                .Setup(m => m.GetRequestResultAsync(correlationId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
