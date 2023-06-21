using AutoMapper;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

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
            .Setup(m => m.GetRequestResultsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<RequestResultModel>());

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
            generator.GenerateStubBasedOnRequestAsync("1", false, CancellationToken.None));
    }

    [TestMethod]
    public async Task GenerateStubBasedOnRequestAsync_RequestFound_SaveStub()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var mapperMock = _mocker.GetMock<IMapper>();
        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var generator = _mocker.CreateInstance<RequestStubGenerator>();

        const string expectedStubId = "generated-b7a0313774b50962789dc0b2841f1458";

        var request =
            new RequestResultModel {CorrelationId = "2", RequestParameters = new RequestParametersModel()};

        stubContextMock
            .Setup(m => m.GetRequestResultAsync("2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        var mappedRequest = new HttpRequestModel();
        mapperMock
            .Setup(m => m.Map<HttpRequestModel>(request.RequestParameters))
            .Returns(mappedRequest);

        var conditions = new StubConditionsModel();
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(mappedRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conditions);

        var fullStub = new FullStubModel();
        stubContextMock
            .Setup(m => m.AddStubAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fullStub);

        // Act
        var result = await generator.GenerateStubBasedOnRequestAsync("2", false, CancellationToken.None);

        // Assert
        Assert.AreEqual(fullStub, result);
        stubContextMock.Verify(m => m.DeleteStubAsync(expectedStubId, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task GenerateStubBasedOnRequestAsync_RequestFound_DoNotSaveStub()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var mapperMock = _mocker.GetMock<IMapper>();
        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var generator = _mocker.CreateInstance<RequestStubGenerator>();

        const string expectedStubId = "generated-b7a0313774b50962789dc0b2841f1458";

        var request =
            new RequestResultModel {CorrelationId = "2", RequestParameters = new RequestParametersModel()};

        stubContextMock
            .Setup(m => m.GetRequestResultAsync("2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        var mappedRequest = new HttpRequestModel();
        mapperMock
            .Setup(m => m.Map<HttpRequestModel>(request.RequestParameters))
            .Returns(mappedRequest);

        var conditions = new StubConditionsModel();
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(mappedRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conditions);

        // Act
        var result = await generator.GenerateStubBasedOnRequestAsync("2", true, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual("OK!", result.Stub.Response.Text);
        Assert.AreEqual(expectedStubId, result.Stub.Id);
        Assert.AreEqual(conditions, result.Stub.Conditions);

        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task GenerateStubBasedOnRequestAsync_RequestFound_ResponseFound()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var mapperMock = _mocker.GetMock<IMapper>();
        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var httpResponseToStubResponseServiceMock = _mocker.GetMock<IHttpResponseToStubResponseService>();
        var generator = _mocker.CreateInstance<RequestStubGenerator>();

        const string expectedStubId = "generated-0d64002c8a32c99f1863f8f748d6dc12";

        var request =
            new RequestResultModel {CorrelationId = "2", RequestParameters = new RequestParametersModel()};

        stubContextMock
            .Setup(m => m.GetRequestResultAsync("2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        var mappedRequest = new HttpRequestModel();
        mapperMock
            .Setup(m => m.Map<HttpRequestModel>(request.RequestParameters))
            .Returns(mappedRequest);

        var conditions = new StubConditionsModel();
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(mappedRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conditions);

        var response = new ResponseModel();
        stubContextMock
            .Setup(m => m.GetResponseAsync("2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        var mappedResponse = new HttpResponseModel();
        mapperMock
            .Setup(m => m.Map<HttpResponseModel>(response))
            .Returns(mappedResponse);
        var stubResponse = new StubResponseModel();
        httpResponseToStubResponseServiceMock
            .Setup(m => m.ConvertToResponseAsync(mappedResponse, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubResponse);

        // Act
        var result = await generator.GenerateStubBasedOnRequestAsync("2", true, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual(stubResponse, result.Stub.Response);
        Assert.AreEqual(expectedStubId, result.Stub.Id);
        Assert.AreEqual(conditions, result.Stub.Conditions);

        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
