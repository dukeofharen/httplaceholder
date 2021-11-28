using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class RequestStubGeneratorFacts
    {
        private readonly AutoMocker _mocker = new();
        private readonly Mock<IRequestToStubConditionsHandler> _mockHandler1 = new();
        private readonly Mock<IRequestToStubConditionsHandler> _mockHandler2 = new();

        [TestInitialize]
        public void Initialize() =>
            _mocker.Use<IEnumerable<IRequestToStubConditionsHandler>>(new[]
            {
                _mockHandler1.Object, _mockHandler2.Object
            });

        [TestCleanup]
        public void Cleanup() => _mocker.VerifyAll();

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_NoRequestFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var requests = new[]
            {
                new RequestResultModel {CorrelationId = "1"}, new RequestResultModel {CorrelationId = "2"},
            };

            var mockStubContext = _mocker.GetMock<IStubContext>();
            var generator = _mocker.CreateInstance<RequestStubGenerator>();

            mockStubContext
                .Setup(m => m.GetRequestResultsAsync())
                .ReturnsAsync(requests);

            // Act / Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                generator.GenerateStubBasedOnRequestAsync("3", false));
        }

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_RequestFound_ShouldCreateAndReturnStub()
        {
            // Arrange
            var requests = new[]
            {
                new RequestResultModel {CorrelationId = "1"}, new RequestResultModel {CorrelationId = "2"},
            };

            var mockMapper = _mocker.GetMock<IMapper>();
            var mockStubContext = _mocker.GetMock<IStubContext>();
            var generator = _mocker.CreateInstance<RequestStubGenerator>();

            var mappedRequest1 = new HttpRequestModel();
            var mappedRequest2 = new HttpRequestModel();
            mockMapper
                .Setup(m => m.Map<HttpRequestModel>(requests[0]))
                .Returns(mappedRequest1);
            mockMapper
                .Setup(m => m.Map<HttpRequestModel>(requests[1]))
                .Returns(mappedRequest2);

            mockStubContext
                .Setup(m => m.GetRequestResultsAsync())
                .ReturnsAsync(requests);

            StubModel actualStub = null;
            var expectedStub = new FullStubModel();
            mockStubContext
                .Setup(m => m.AddStubAsync(It.IsAny<StubModel>()))
                .Callback<StubModel>(stub => actualStub = stub)
                .ReturnsAsync(expectedStub);

            const string expectedId = "generated-0876599ae8c21242dd796e82c89217ee";

            // Act
            var result = await generator.GenerateStubBasedOnRequestAsync("2", false);

            // Assert
            _mockHandler1.Verify(m => m.HandleStubGenerationAsync(mappedRequest1, It.IsAny<StubConditionsModel>()));
            _mockHandler2.Verify(m => m.HandleStubGenerationAsync(mappedRequest1, It.IsAny<StubConditionsModel>()));

            mockStubContext.Verify(m => m.DeleteStubAsync(expectedId));

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

            var mockMapper = _mocker.GetMock<IMapper>();
            var mockStubContext = _mocker.GetMock<IStubContext>();
            var generator = _mocker.CreateInstance<RequestStubGenerator>();

            var mappedRequest1 = new HttpRequestModel();
            var mappedRequest2 = new HttpRequestModel();
            mockMapper
                .Setup(m => m.Map<HttpRequestModel>(requests[0]))
                .Returns(mappedRequest1);
            mockMapper
                .Setup(m => m.Map<HttpRequestModel>(requests[1]))
                .Returns(mappedRequest2);

            mockStubContext
                .Setup(m => m.GetRequestResultsAsync())
                .ReturnsAsync(requests);

            const string expectedId = "generated-0876599ae8c21242dd796e82c89217ee";

            // Act
            var result = await generator.GenerateStubBasedOnRequestAsync("2", true);

            // Assert
            _mockHandler1.Verify(m => m.HandleStubGenerationAsync(mappedRequest1, It.IsAny<StubConditionsModel>()));
            _mockHandler2.Verify(m => m.HandleStubGenerationAsync(mappedRequest1, It.IsAny<StubConditionsModel>()));

            mockStubContext.Verify(m => m.DeleteStubAsync(expectedId), Times.Never);
            mockStubContext.Verify(m => m.AddStubAsync(It.IsAny<StubModel>()), Times.Never);

            Assert.AreEqual(expectedId, result.Stub.Id);
            Assert.AreEqual("OK!", result.Stub.Response.Text);
        }
    }
}
