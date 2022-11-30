using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.Tests.StubExecution.OpenAPIParsing;

[TestClass]
public class OpenApiToStubConverterFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ConvertToStubAsync_HappyFlow()
    {
        // Arrange
        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var httpResponseToStubResponseServiceMock = _mocker.GetMock<IHttpResponseToStubResponseService>();
        var openApiDataFillerMock = _mocker.GetMock<IOpenApiDataFiller>();
        var converter = _mocker.CreateInstance<OpenApiToStubConverter>();

        var line = new OpenApiLine
        {
            Operation = new OpenApiOperation {Summary = "API to get users"},
            OperationType = OperationType.Post,
            PathKey = "/users",
            Response = new OpenApiResponse(),
            ResponseKey = "200"
        };
        const string serverUrl = "http://localhost";
        var server = new OpenApiServer {Url = serverUrl};
        const string tenant = "tenant1";

        openApiDataFillerMock
            .Setup(m => m.BuildServerUrl(server))
            .Returns(serverUrl);

        const string requestBody = "request body";
        openApiDataFillerMock
            .Setup(m => m.BuildRequestBody(line.Operation))
            .Returns(requestBody);

        var requestHeaders = new Dictionary<string, string>();
        openApiDataFillerMock
            .Setup(m => m.BuildRequestHeaders(line.Operation))
            .Returns(requestHeaders);

        const string relativePath = "/users";
        openApiDataFillerMock
            .Setup(m => m.BuildRelativeRequestPath(line.Operation, line.PathKey))
            .Returns(relativePath);

        const string responseBody = "response body";
        openApiDataFillerMock
            .Setup(m => m.BuildResponseBody(line.Response))
            .Returns(responseBody);

        var responseHeaders = new Dictionary<string, string>();
        openApiDataFillerMock
            .Setup(m => m.BuildResponseHeaders(line.Response))
            .Returns(responseHeaders);

        const int statusCode = 200;
        openApiDataFillerMock
            .Setup(m => m.BuildHttpStatusCode(line.ResponseKey))
            .Returns(statusCode);

        var conditions = new StubConditionsModel();
        HttpRequestModel capturedRequest = null;
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(It.IsAny<HttpRequestModel>(), It.IsAny<CancellationToken>()))
            .Callback<HttpRequestModel, CancellationToken>((r, _) => capturedRequest = r)
            .ReturnsAsync(conditions);

        var stubResponse = new StubResponseModel();
        HttpResponseModel capturedResponse = null;
        httpResponseToStubResponseServiceMock
            .Setup(m => m.ConvertToResponseAsync(It.IsAny<HttpResponseModel>(), It.IsAny<CancellationToken>()))
            .Callback<HttpResponseModel, CancellationToken>((r, _) => capturedResponse = r)
            .ReturnsAsync(stubResponse);

        // Act
        var result = await converter.ConvertToStubAsync(server, line, tenant, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(conditions, result.Conditions);
        Assert.AreEqual(stubResponse, result.Response);
        Assert.AreEqual(tenant, result.Tenant);
        Assert.AreEqual("API to get users", result.Description);
        Assert.AreEqual("generated-10beced6f421e77a254aec44c1e51d95", result.Id);

        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual(requestBody, capturedRequest.Body);
        Assert.AreEqual(requestHeaders, capturedRequest.Headers);
        Assert.AreEqual("POST", capturedRequest.Method);
        Assert.AreEqual("http://localhost/users", capturedRequest.Url);

        Assert.IsNotNull(capturedResponse);
        Assert.AreEqual(responseBody, capturedResponse.Content);
        Assert.AreEqual(responseHeaders, capturedResponse.Headers);
        Assert.AreEqual(statusCode, capturedResponse.StatusCode);
    }
}
