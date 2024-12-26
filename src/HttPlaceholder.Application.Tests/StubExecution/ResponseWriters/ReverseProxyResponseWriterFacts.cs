using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Common.Utilities;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class ReverseProxyResponseWriterFacts
{
    private readonly AutoMocker _mocker = new();

    [TestMethod]
    public async Task WriteToResponseAsync_NoProxySet_ShouldReturnFalse()
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var stub = new StubModel { Response = new StubResponseModel { ReverseProxy = null } };

        // Act
        var result = await writer.WriteToResponseAsync(stub, null, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_NoProxyUrlSet_ShouldReturnFalse()
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel { Url = string.Empty }
            }
        };

        // Act
        var result = await writer.WriteToResponseAsync(stub, null, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [DataTestMethod]
    [DataRow(
        "https://todo.com",
        "/todoitems",
        "?key=val",
        "/todoitems/todos/1",
        false,
        true,
        "https://todo.com/todos/1")]
    [DataRow(
        "https://todo.com/todos/1",
        "",
        "?key=val",
        "/todos/1",
        false,
        false,
        "https://todo.com/todos/1")]
    [DataRow(
        "https://todo.com/todos/1",
        "",
        "?key=val",
        "/todos/1",
        true,
        false,
        "https://todo.com/todos/1?key=val")]
    [DataRow(
        "https://ducode.org/",
        "/ducode",
        "",
        "/ducode",
        true,
        true,
        "https://ducode.org/")]
    public async Task WriteToResponseAsync_RequestUrlShouldBeSetCorrectly(
        string proxyUrl,
        string pathCondition,
        string queryString,
        string path,
        bool appendQueryString,
        bool appendPath,
        string expectedRequestUrl)
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var mockHttpClientFactory = _mocker.GetMock<IHttpClientFactory>();
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = pathCondition } },
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = appendPath, AppendQueryString = appendQueryString, Url = proxyUrl
                }
            }
        };
        mockHttpContextService
            .Setup(m => m.Path)
            .Returns(path);
        mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");
        mockHttpContextService
            .Setup(m => m.GetQueryString())
            .Returns(queryString);
        mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string>());

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, expectedRequestUrl)
            .Respond(MimeTypes.TextMime, "OK");
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        SetupHostnameIsValid(proxyUrl, true);

        var responseModel = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_RequestHeadersShouldBeSetCorrectly()
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var mockHttpClientFactory = _mocker.GetMock<IHttpClientFactory>();
        const string proxyUrl = "http://example.com";
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = proxyUrl
                }
            }
        };
        mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");

        var headers = new Dictionary<string, string>
        {
            { HeaderKeys.ContentType, MimeTypes.JsonMime },
            { HeaderKeys.ContentLength, "111" },
            { HeaderKeys.Host, "localhost:5000" },
            { "X-Api-Key", "abc123" },
            { "Connection", "keep-alive" },
            { HeaderKeys.AcceptEncoding, "utf-8" },
            { "Accept", MimeTypes.JsonMime },
            { "User-Agent", "WordPress/6.2.2; http://localhost:8010" }
        };
        mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(proxyUrl)
            .With(r =>
            {
                var proxyHeaders = r.Headers.ToDictionary(h => h.Key, h => h.Value.First());
                return proxyHeaders.Count == 3 &&
                       proxyHeaders["X-Api-Key"] == "abc123" &&
                       proxyHeaders["Accept"] == MimeTypes.JsonMime;
            })
            .Respond(MimeTypes.TextMime, "OK");
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        SetupHostnameIsValid(proxyUrl, true);

        var responseModel = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ResponseHeadersShouldBeSetCorrectly()
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var mockHttpClientFactory = _mocker.GetMock<IHttpClientFactory>();
        const string proxyUrl = "http://example.com";
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = proxyUrl
                }
            }
        };
        mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");

        mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string>());

        var mockHttp = new MockHttpMessageHandler();
        var responseHeaders = new Dictionary<string, string>
        {
            { "Token", "abc123" },
            { "Some-Date", "2020-08-16" },
            { HeaderKeys.XHttPlaceholderCorrelation, "correlation" },
            { HeaderKeys.XHttPlaceholderExecutedStub, "stub-id" },
            { HeaderKeys.TransferEncoding, "chunked" }
        };
        mockHttp
            .When(proxyUrl)
            .Respond(_ =>
            {
                var msg = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("OK", Encoding.UTF8, MimeTypes.TextMime)
                };
                foreach (var (key, value) in responseHeaders)
                {
                    msg.Headers.Add(key, value);
                }

                return msg;
            });
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());
        SetupHostnameIsValid(proxyUrl, true);

        var responseModel = new ResponseModel();
        responseModel.Headers.Add("Some-Date", "2020-08-11");

        // Act
        var result = await writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);

        Assert.AreEqual(3, responseModel.Headers.Count);
        Assert.AreEqual($"{MimeTypes.TextMime}; charset=utf-8", responseModel.Headers[HeaderKeys.ContentType]);
        Assert.AreEqual("2020-08-16", responseModel.Headers["Some-Date"]);
        Assert.AreEqual("abc123", responseModel.Headers["Token"]);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_PostData_ShouldSendContentAndContentTypeCorrectly()
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var mockHttpClientFactory = _mocker.GetMock<IHttpClientFactory>();
        const string proxyUrl = "http://example.com";
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = proxyUrl
                }
            }
        };
        SetupHostnameIsValid(proxyUrl, true);
        mockHttpContextService
            .Setup(m => m.Method)
            .Returns("POST");

        const string body = "{\"key\": \"val\"}";
        mockHttpContextService
            .Setup(m => m.GetBodyAsBytesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(body));

        mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string> { { HeaderKeys.ContentType, MimeTypes.JsonMime } });

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, proxyUrl)
            .With(r =>
            {
                var contentHeaders = r.Content.Headers.ToDictionary(h => h.Key, h => h.Value.First());
                return contentHeaders.Count == 1 &&
                       contentHeaders[HeaderKeys.ContentType] == MimeTypes.JsonMime;
            })
            .Respond(MimeTypes.TextMime, "OK");
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        var responseModel = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ShouldReplaceRootUrlInContent()
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var mockHttpClientFactory = _mocker.GetMock<IHttpClientFactory>();
        const string proxyUrl = "http://example.com";
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = proxyUrl
                }
            }
        };
        mockHttpContextService
            .Setup(m => m.Method)
            .Returns("POST");

        const string body = "{\"key\": \"val\"}";
        mockHttpContextService
            .Setup(m => m.GetBodyAsBytesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(body));

        mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string> { { HeaderKeys.ContentType, MimeTypes.JsonMime } });

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, proxyUrl)
            .With(r =>
            {
                var contentHeaders = r.Content.Headers.ToDictionary(h => h.Key, h => h.Value.First());
                return contentHeaders.Count == 1 &&
                       contentHeaders[HeaderKeys.ContentType] == MimeTypes.JsonMime;
            })
            .Respond(MimeTypes.TextMime, "OK");
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());
        SetupHostnameIsValid(proxyUrl, true);

        var responseModel = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsFalse(responseModel.BodyIsBinary);
        Assert.IsTrue(result.Executed);
    }

    [DataTestMethod]
    [DataRow(
        "https://todo.com",
        "/todoitems",
        "/todoitems/todos/1",
        true,
        "http://localhost:5000/todoitems")]
    [DataRow(
        "https://todo.com",
        "/todoitems",
        "/todoitems/todos/1",
        false,
        "http://localhost:5000")]
    public async Task WriteToResponseAsync_ReplaceRootUrlInContent_ShouldBeReplacedCorrectly(
        string proxyUrl,
        string pathCondition,
        string path,
        bool appendPath,
        string expectedReplacementUrl)
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var urlResolverMock = _mocker.GetMock<IUrlResolver>();
        var mockHttpClientFactory = _mocker.GetMock<IHttpClientFactory>();
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = pathCondition } },
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = appendPath,
                    AppendQueryString = false,
                    Url = proxyUrl,
                    ReplaceRootUrl = true
                }
            }
        };
        mockHttpContextService
            .Setup(m => m.Path)
            .Returns(path);
        mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");
        mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string>());
        urlResolverMock
            .Setup(m => m.GetRootUrl())
            .Returns("http://localhost:5000");

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, $"{proxyUrl}/**")
            .Respond(
                HttpStatusCode.OK,
                new Dictionary<string, string> { { "X-Url", proxyUrl } },
                MimeTypes.TextMime,
                proxyUrl);

        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        SetupHostnameIsValid(proxyUrl, true);

        var responseModel = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedReplacementUrl, Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(expectedReplacementUrl, responseModel.Headers["X-Url"]);
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ErrorWhenCallingHttpClient_ShouldReturn502()
    {
        // Arrange
        var writer = _mocker.CreateInstance<ReverseProxyResponseWriter>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var mockHttpClientFactory = _mocker.GetMock<IHttpClientFactory>();
        var urlResolverMock = _mocker.GetMock<IUrlResolver>();

        const string proxyUrl = "https://ducode.org";
        const string path = "/todoitems/todos/1";
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = "/proxy" } },
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = true, AppendQueryString = false, Url = proxyUrl, ReplaceRootUrl = true
                }
            }
        };
        mockHttpContextService
            .Setup(m => m.Path)
            .Returns(path);
        mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");
        mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string>());
        urlResolverMock
            .Setup(m => m.GetRootUrl())
            .Returns("http://localhost:5000");
        SetupHostnameIsValid(proxyUrl, true);

        var client = new HttpClient(new ErrorHttpMessageHandler());
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(client);

        var responseModel = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Log.Contains("ERROR!"));
        Assert.AreEqual((int)HttpStatusCode.BadGateway, responseModel.StatusCode);
        Assert.AreEqual("502 Bad Gateway", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(MimeTypes.TextMime, responseModel.Headers["Content-Type"]);
        Assert.IsFalse(responseModel.BodyIsBinary);
    }

    [TestMethod]
    public void GetPath_PathIsNull_ShouldReturnNull()
    {
        // Arrange
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = null } }
        };

        // Act
        var result = ReverseProxyResponseWriter.GetPath(stub);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetPath_PathIsString_ShouldReturnPath()
    {
        // Arrange
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = "/path" } }
        };

        // Act
        var result = ReverseProxyResponseWriter.GetPath(stub);

        // Assert
        Assert.AreEqual("/path", result);
    }

    [TestMethod]
    public void GetPath_PathIsModel_StringEquals()
    {
        // Arrange
        var path = new StubConditionStringCheckingModel { StringEquals = "/path" };
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = path } }
        };

        // Act
        var result = ReverseProxyResponseWriter.GetPath(stub);

        // Assert
        Assert.AreEqual("/path", result);
    }

    [TestMethod]
    public void GetPath_PathIsModel_StringEqualsCi()
    {
        // Arrange
        var path = new StubConditionStringCheckingModel { StringEqualsCi = "/path" };
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = path } }
        };

        // Act
        var result = ReverseProxyResponseWriter.GetPath(stub);

        // Assert
        Assert.AreEqual("/path", result);
    }

    [TestMethod]
    public void GetPath_PathIsModel_StartsWith()
    {
        // Arrange
        var path = new StubConditionStringCheckingModel { StartsWith = "/path" };
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = path } }
        };

        // Act
        var result = ReverseProxyResponseWriter.GetPath(stub);

        // Assert
        Assert.AreEqual("/path", result);
    }

    [TestMethod]
    public void GetPath_PathIsModel_StartsWithCi()
    {
        // Arrange
        var path = new StubConditionStringCheckingModel { StartsWithCi = "/path" };
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = path } }
        };

        // Act
        var result = ReverseProxyResponseWriter.GetPath(stub);

        // Assert
        Assert.AreEqual("/path", result);
    }

    private void SetupHostnameIsValid(string url, bool isValid)
    {
        var host = UrlHelper.GetHostname(url);
        _mocker.GetMock<IHostnameValidator>()
            .Setup(m => m.HostnameIsValid(host))
            .Returns(isValid);
    }

    private class ErrorHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            throw new Exception("ERROR!");
    }
}
