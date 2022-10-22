using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class ReverseProxyResponseWriterFacts
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory = new();
    private readonly Mock<IHttpContextService> _mockHttpContextService = new();
    private ReverseProxyResponseWriter _writer;

    [TestInitialize]
    public void Initialize() =>
        _writer = new ReverseProxyResponseWriter(_mockHttpClientFactory.Object, _mockHttpContextService.Object);

    [TestCleanup]
    public void Cleanup() => _mockHttpClientFactory.VerifyAll();

    [TestMethod]
    public async Task WriteToResponseAsync_NoProxySet_ShouldReturnFalse()
    {
        // Arrange
        var stub = new StubModel {Response = new StubResponseModel {ReverseProxy = null}};

        // Act
        var result = await _writer.WriteToResponseAsync(stub, null, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_NoProxyUrlSet_ShouldReturnFalse()
    {
        // Arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel {ReverseProxy = new StubResponseReverseProxyModel {Url = string.Empty}}
        };

        // Act
        var result = await _writer.WriteToResponseAsync(stub, null, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [DataTestMethod]
    [DataRow(
        "https://jsonplaceholder.typicode.com",
        "/todoitems",
        "?key=val",
        "/todoitems/todos/1",
        false,
        true,
        "https://jsonplaceholder.typicode.com/todos/1")]
    [DataRow(
        "https://jsonplaceholder.typicode.com/todos/1",
        "",
        "?key=val",
        "/todos/1",
        false,
        false,
        "https://jsonplaceholder.typicode.com/todos/1")]
    [DataRow(
        "https://jsonplaceholder.typicode.com/todos/1",
        "",
        "?key=val",
        "/todos/1",
        true,
        false,
        "https://jsonplaceholder.typicode.com/todos/1?key=val")]
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
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel {Url = new StubUrlConditionModel {Path = pathCondition}},
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = appendPath, AppendQueryString = appendQueryString, Url = proxyUrl
                }
            }
        };
        _mockHttpContextService
            .Setup(m => m.Path)
            .Returns(path);
        _mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");
        _mockHttpContextService
            .Setup(m => m.GetQueryString())
            .Returns(queryString);
        _mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string>());

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, expectedRequestUrl)
            .Respond(Constants.TextMime, "OK");
        _mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        var responseModel = new ResponseModel();

        // Act
        var result = await _writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_RequestHeadersShouldBeSetCorrectly()
    {
        // Arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = "http://example.com"
                }
            }
        };
        _mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");

        var headers = new Dictionary<string, string>
        {
            {"Content-Type", Constants.JsonMime},
            {"Content-Length", "111"},
            {"Host", "localhost:5000"},
            {"X-Api-Key", "abc123"},
            {"Connection", "keep-alive"},
            {"Accept-Encoding", "utf-8"},
            {"Accept", Constants.JsonMime}
        };
        _mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When("http://example.com")
            .With(r =>
            {
                var proxyHeaders = r.Headers.ToDictionary(h => h.Key, h => h.Value.First());
                return proxyHeaders.Count == 2 &&
                       proxyHeaders["X-Api-Key"] == "abc123" &&
                       proxyHeaders["Accept"] == Constants.JsonMime;
            })
            .Respond(Constants.TextMime, "OK");
        _mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        var responseModel = new ResponseModel();

        // Act
        var result = await _writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ResponseHeadersShouldBeSetCorrectly()
    {
        // Arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = "http://example.com"
                }
            }
        };
        _mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");

        _mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string>());

        var mockHttp = new MockHttpMessageHandler();
        var responseHeaders = new Dictionary<string, string>
        {
            {"Token", "abc123"},
            {"Some-Date", "2020-08-16"},
            {"X-HttPlaceholder-Correlation", "correlation"},
            {"X-HttPlaceholder-ExecutedStub", "stub-id"},
            {"Transfer-Encoding", "chunked"}
        };
        mockHttp
            .When("http://example.com")
            .Respond(_ =>
            {
                var msg = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("OK", Encoding.UTF8, Constants.TextMime)
                };
                foreach (var (key, value) in responseHeaders)
                {
                    msg.Headers.Add(key, value);
                }

                return msg;
            });
        _mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        var responseModel = new ResponseModel();
        responseModel.Headers.Add("Some-Date", "2020-08-11");

        // Act
        var result = await _writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);

        Assert.AreEqual(3, responseModel.Headers.Count);
        Assert.AreEqual($"{Constants.TextMime}; charset=utf-8", responseModel.Headers["Content-Type"]);
        Assert.AreEqual("2020-08-16", responseModel.Headers["Some-Date"]);
        Assert.AreEqual("abc123", responseModel.Headers["Token"]);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_PostData_ShouldSendContentAndContentTypeCorrectly()
    {
        // Arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = "http://example.com"
                }
            }
        };
        _mockHttpContextService
            .Setup(m => m.Method)
            .Returns("POST");

        const string body = "{\"key\": \"val\"}";
        _mockHttpContextService
            .Setup(m => m.GetBodyAsBytes())
            .Returns(Encoding.UTF8.GetBytes(body));

        _mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string> {{"Content-Type", Constants.JsonMime}});

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, "http://example.com")
            .With(r =>
            {
                var contentHeaders = r.Content.Headers.ToDictionary(h => h.Key, h => h.Value.First());
                return contentHeaders.Count == 1 &&
                       contentHeaders["Content-Type"] == Constants.JsonMime;
            })
            .Respond(Constants.TextMime, "OK");
        _mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        var responseModel = new ResponseModel();

        // Act
        var result = await _writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ShouldReplaceRootUrlInContent()
    {
        // Arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    AppendPath = false, AppendQueryString = false, Url = "http://example.com"
                }
            }
        };
        _mockHttpContextService
            .Setup(m => m.Method)
            .Returns("POST");

        const string body = "{\"key\": \"val\"}";
        _mockHttpContextService
            .Setup(m => m.GetBodyAsBytes())
            .Returns(Encoding.UTF8.GetBytes(body));

        _mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string> {{"Content-Type", Constants.JsonMime}});

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, "http://example.com")
            .With(r =>
            {
                var contentHeaders = r.Content.Headers.ToDictionary(h => h.Key, h => h.Value.First());
                return contentHeaders.Count == 1 &&
                       contentHeaders["Content-Type"] == Constants.JsonMime;
            })
            .Respond(Constants.TextMime, "OK");
        _mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        var responseModel = new ResponseModel();

        // Act
        var result = await _writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual("OK", Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }

    [DataTestMethod]
    [DataRow(
        "https://jsonplaceholder.typicode.com",
        "/todoitems",
        "/todoitems/todos/1",
        true,
        "http://localhost:5000/todoitems")]
    [DataRow(
        "https://jsonplaceholder.typicode.com",
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
        var stub = new StubModel
        {
            Conditions = new StubConditionsModel {Url = new StubUrlConditionModel {Path = pathCondition}},
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
        _mockHttpContextService
            .Setup(m => m.Path)
            .Returns(path);
        _mockHttpContextService
            .Setup(m => m.Method)
            .Returns("GET");
        _mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(new Dictionary<string, string>());
        _mockHttpContextService
            .Setup(m => m.RootUrl)
            .Returns("http://localhost:5000");

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, $"{proxyUrl}/**")
            .Respond(
                HttpStatusCode.OK,
                new Dictionary<string, string> {{"X-Url", proxyUrl}},
                Constants.TextMime,
                proxyUrl);

        _mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(mockHttp.ToHttpClient());

        var responseModel = new ResponseModel();

        // Act
        var result = await _writer.WriteToResponseAsync(stub, responseModel, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedReplacementUrl, Encoding.UTF8.GetString(responseModel.Body));
        Assert.AreEqual(expectedReplacementUrl, responseModel.Headers["X-Url"]);
        Assert.AreEqual(200, responseModel.StatusCode);
        Assert.IsTrue(result.Executed);
    }
}
