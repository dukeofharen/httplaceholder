using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class RequestStubGeneratorFacts
    {
        private readonly Mock<ILogger<RequestStubGenerator>> _mockLogger = new();
        private readonly Mock<IStubContext> _mockStubContext = new();
        private readonly Mock<IRequestStubGenerationHandler> _mockHandler1 = new();
        private readonly Mock<IRequestStubGenerationHandler> _mockHandler2 = new();
        private RequestStubGenerator _generator;

        [TestInitialize]
        public void Initialize() => _generator = new RequestStubGenerator(
            _mockStubContext.Object,
            new[] {_mockHandler1.Object, _mockHandler2.Object},
            _mockLogger.Object);

        [TestCleanup]
        public void Cleanup()
        {
            _mockLogger.VerifyAll();
            _mockStubContext.VerifyAll();
            _mockHandler1.VerifyAll();
            _mockHandler2.VerifyAll();
        }

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_NoRequestFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var requests = new[]
            {
                new RequestResultModel {CorrelationId = "1"}, new RequestResultModel {CorrelationId = "2"},
            };

            _mockStubContext
                .Setup(m => m.GetRequestResultsAsync())
                .ReturnsAsync(requests);

            // Act / Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                _generator.GenerateStubBasedOnRequestAsync("3", false));
        }

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_RequestFound_ShouldCreateAndReturnStub()
        {
            // Arrange
            var requests = new[]
            {
                new RequestResultModel {CorrelationId = "1"}, new RequestResultModel {CorrelationId = "2"},
            };

            _mockStubContext
                .Setup(m => m.GetRequestResultsAsync())
                .ReturnsAsync(requests);

            StubModel actualStub = null;
            var expectedStub = new FullStubModel();
            _mockStubContext
                .Setup(m => m.AddStubAsync(It.IsAny<StubModel>()))
                .Callback<StubModel>(stub => actualStub = stub)
                .ReturnsAsync(expectedStub);

            const string expectedId = "generated-0876599ae8c21242dd796e82c89217ee";

            // Act
            var result = await _generator.GenerateStubBasedOnRequestAsync("2", false);

            // Assert
            _mockHandler1.Verify(m => m.HandleStubGenerationAsync(requests[1], It.IsAny<StubModel>()));
            _mockHandler2.Verify(m => m.HandleStubGenerationAsync(requests[1], It.IsAny<StubModel>()));

            _mockStubContext.Verify(m => m.DeleteStubAsync(expectedId));

            Assert.AreEqual(expectedStub, result);
            Assert.IsNotNull(actualStub);
            Assert.AreEqual(expectedId, actualStub.Id);
            Assert.AreEqual("OK!", actualStub.Response.Text);
        }

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_RequestFound_DoNotCreateStub_OnlyReturnStub()
        {
            // Arrange
            var requests = new[]
            {
                new RequestResultModel {CorrelationId = "1"}, new RequestResultModel {CorrelationId = "2"},
            };

            _mockStubContext
                .Setup(m => m.GetRequestResultsAsync())
                .ReturnsAsync(requests);

            const string expectedId = "generated-0876599ae8c21242dd796e82c89217ee";

            // Act
            var result = await _generator.GenerateStubBasedOnRequestAsync("2", true);

            // Assert
            _mockHandler1.Verify(m => m.HandleStubGenerationAsync(requests[1], It.IsAny<StubModel>()));
            _mockHandler2.Verify(m => m.HandleStubGenerationAsync(requests[1], It.IsAny<StubModel>()));

            _mockStubContext.Verify(m => m.DeleteStubAsync(expectedId), Times.Never);
            _mockStubContext.Verify(m => m.AddStubAsync(It.IsAny<StubModel>()), Times.Never);

            Assert.AreEqual(expectedId, result.Stub.Id);
            Assert.AreEqual("OK!", result.Stub.Response.Text);
        }
    }
}
