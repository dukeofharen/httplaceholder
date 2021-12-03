using System;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;
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

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_RequestNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var stubContextMock = _mocker.GetMock<IStubContext>();
            var generator = _mocker.CreateInstance<RequestStubGenerator>();

            stubContextMock
                .Setup(m => m.GetRequestResultsAsync())
                .ReturnsAsync(Array.Empty<RequestResultModel>());

            // Act / Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                generator.GenerateStubBasedOnRequestAsync("1", false));
        }

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_RequestFound_SaveStub()
        {
            // Arrange
            var stubContextMock = _mocker.GetMock<IStubContext>();
            var mapperMock = _mocker.GetMock<IMapper>();
            var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
            var generator = _mocker.CreateInstance<RequestStubGenerator>();

            const string expectedStubId = "generated-0876599ae8c21242dd796e82c89217ee";

            var request =
                new RequestResultModel { CorrelationId = "2", RequestParameters = new RequestParametersModel() };

            stubContextMock
                .Setup(m => m.GetRequestResultAsync("2"))
                .ReturnsAsync(request);

            var mappedRequest = new HttpRequestModel();
            mapperMock
                .Setup(m => m.Map<HttpRequestModel>(request.RequestParameters))
                .Returns(mappedRequest);

            var conditions = new StubConditionsModel();
            httpRequestToConditionsServiceMock
                .Setup(m => m.ConvertToConditionsAsync(mappedRequest))
                .ReturnsAsync(conditions);

            var fullStub = new FullStubModel();
            stubContextMock
                .Setup(m => m.AddStubAsync(It.IsAny<StubModel>()))
                .ReturnsAsync(fullStub);

            // Act
            var result = await generator.GenerateStubBasedOnRequestAsync("2", false);

            // Assert
            Assert.AreEqual(fullStub, result);
            stubContextMock.Verify(m => m.DeleteStubAsync(expectedStubId));
        }

        [TestMethod]
        public async Task GenerateStubBasedOnRequestAsync_RequestFound_DoNotSaveStub()
        {
            // Arrange
            var stubContextMock = _mocker.GetMock<IStubContext>();
            var mapperMock = _mocker.GetMock<IMapper>();
            var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
            var generator = _mocker.CreateInstance<RequestStubGenerator>();

            const string expectedStubId = "generated-0876599ae8c21242dd796e82c89217ee";

            var request =
                new RequestResultModel { CorrelationId = "2", RequestParameters = new RequestParametersModel() };

            stubContextMock
                .Setup(m => m.GetRequestResultAsync("2"))
                .ReturnsAsync(request);

            var mappedRequest = new HttpRequestModel();
            mapperMock
                .Setup(m => m.Map<HttpRequestModel>(request.RequestParameters))
                .Returns(mappedRequest);

            var conditions = new StubConditionsModel();
            httpRequestToConditionsServiceMock
                .Setup(m => m.ConvertToConditionsAsync(mappedRequest))
                .ReturnsAsync(conditions);

            // Act
            var result = await generator.GenerateStubBasedOnRequestAsync("2", true);

            // Assert
            Assert.IsNotNull(result.Metadata);
            Assert.AreEqual("OK!", result.Stub.Response.Text);
            Assert.AreEqual(expectedStubId, result.Stub.Id);
            Assert.AreEqual(conditions, result.Stub.Conditions);

            stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>()), Times.Never);
            stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>()), Times.Never);
        }
    }
}
