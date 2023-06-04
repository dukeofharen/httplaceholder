using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class HarStubGeneratorFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task GenerateHarStubsAsync_HarIsNull_ShouldThrowValidationException()
    {
        // Arrange
        const string input = "null";
        var generator = _mocker.CreateInstance<HarStubGenerator>();

        // Act
        var exception = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
            generator.GenerateStubsAsync(input, false, null, null, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("The HAR was invalid."));
    }

    [TestMethod]
    public async Task GenerateHarStubsAsync_HarLogIsNull_ShouldThrowValidationException()
    {
        // Arrange
        var input = await File.ReadAllTextAsync("Resources/HAR/har_no_log.json");
        var generator = _mocker.CreateInstance<HarStubGenerator>();

        // Act
        var exception = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
            generator.GenerateStubsAsync(input, false, null, null, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("har.log is not set."));
    }

    [TestMethod]
    public async Task GenerateHarStubsAsync_EntriesIsNull_ShouldThrowValidationException()
    {
        // Arrange
        var input = await File.ReadAllTextAsync("Resources/HAR/har_entries_null.json");
        var generator = _mocker.CreateInstance<HarStubGenerator>();

        // Act
        var exception = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
            generator.GenerateStubsAsync(input, false, null, null, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("No entries set in HAR."));
    }

    [TestMethod]
    public async Task GenerateHarStubsAsync_EntriesIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var input = await File.ReadAllTextAsync("Resources/HAR/har_entries_empty.json");
        var generator = _mocker.CreateInstance<HarStubGenerator>();

        // Act
        var exception = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
            generator.GenerateStubsAsync(input, false, null, null, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("No entries set in HAR."));
    }

    [TestMethod]
    public async Task GenerateHarStubsAsync_HappyFlow()
    {
        // Arrange
        var input = await File.ReadAllTextAsync("Resources/HAR/har_regular.json");
        var generator = _mocker.CreateInstance<HarStubGenerator>();

        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var httpResponseToStubResponseServiceMock = _mocker.GetMock<IHttpResponseToStubResponseService>();
        var stubContextMock = _mocker.GetMock<IStubContext>();

        var requests = new List<HttpRequestModel>();
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(It.IsAny<HttpRequestModel>(), It.IsAny<CancellationToken>()))
            .Callback<HttpRequestModel, CancellationToken>((r, _) => requests.Add(r))
            .ReturnsAsync(new StubConditionsModel());

        var responses = new List<HttpResponseModel>();
        httpResponseToStubResponseServiceMock
            .Setup(m => m.ConvertToResponseAsync(It.IsAny<HttpResponseModel>(), It.IsAny<CancellationToken>()))
            .Callback<HttpResponseModel, CancellationToken>((r, _) => responses.Add(r))
            .ReturnsAsync(new StubResponseModel());

        // Act
        var result = (await generator.GenerateStubsAsync(input, false, null, "prefix", CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(3, requests.Count);
        Assert.AreEqual(3, responses.Count);

        var req1 = requests[0];
        Assert.AreEqual("GET", req1.Method);
        Assert.AreEqual("https://ducode.org/", req1.Url);
        Assert.AreEqual(14, req1.Headers.Count);
        Assert.AreEqual("no-cache", req1.Headers["pragma"]);
        Assert.IsNull(req1.ClientIp);
        Assert.IsNull(req1.Body);

        var res1 = responses[0];
        Assert.IsTrue(res1.Content.Contains("<!DOCTYPE html>"));
        Assert.IsFalse(res1.ContentIsBase64);
        Assert.AreEqual(200, res1.StatusCode);
        Assert.AreEqual(5, res1.Headers.Count);
        Assert.IsFalse(res1.Headers.Any(h =>
            h.Key.Equals(HeaderKeys.ContentLength.ToLower(), StringComparison.OrdinalIgnoreCase)));
        Assert.IsFalse(res1.Headers.Any(h => h.Key.Equals("content-encoding", StringComparison.OrdinalIgnoreCase)));
        Assert.AreEqual(MimeTypes.HtmlMime, res1.Headers[HeaderKeys.ContentType.ToLower()]);

        var req2 = requests[1];
        Assert.AreEqual("GET", req2.Method);
        Assert.AreEqual("https://ducode.org/static/style/style.css", req2.Url);
        Assert.AreEqual(13, req2.Headers.Count);
        Assert.AreEqual("no-cache", req2.Headers["pragma"]);
        Assert.IsNull(req2.ClientIp);
        Assert.IsNull(req2.Body);

        var res2 = responses[1];
        Assert.IsTrue(res2.Content.Contains("QGZvbnQtZmFjZSB7CiAgICBmb250LWZhb"));
        Assert.IsTrue(res2.ContentIsBase64);
        Assert.AreEqual(200, res2.StatusCode);
        Assert.AreEqual(6, res2.Headers.Count);
        Assert.AreEqual("text/css", res2.Headers[HeaderKeys.ContentType.ToLower()]);

        var req3 = requests[2];
        Assert.AreEqual("PUT", req3.Method);
        Assert.AreEqual("https://api.site.com/api/v1/admin/users/123", req3.Url);
        Assert.AreEqual(16, req3.Headers.Count);
        Assert.AreEqual("Bearer bearer", req3.Headers["Authorization"]);
        Assert.IsNull(req3.ClientIp);
        Assert.AreEqual("{\"firstName\":\"Dukeofharen\"}", req3.Body);

        var res3 = responses[2];
        Assert.IsTrue(string.IsNullOrWhiteSpace(res3.Content));
        Assert.IsFalse(res3.ContentIsBase64);
        Assert.AreEqual(204, res3.StatusCode);
        Assert.AreEqual(11, res3.Headers.Count);
        Assert.AreEqual("h2", res3.Headers["X-Firefox-Spdy"]);

        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [TestMethod]
    public async Task GenerateHarStubsAsync_HappyFlow_DoNotCreateStub()
    {
        // Arrange
        var input = await File.ReadAllTextAsync("Resources/HAR/har_regular.json");
        var generator = _mocker.CreateInstance<HarStubGenerator>();

        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var httpResponseToStubResponseServiceMock = _mocker.GetMock<IHttpResponseToStubResponseService>();
        var stubContextMock = _mocker.GetMock<IStubContext>();

        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(It.IsAny<HttpRequestModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StubConditionsModel {Method = "GET", Url = new StubUrlConditionModel {Path = "/path1"}});

        httpResponseToStubResponseServiceMock
            .Setup(m => m.ConvertToResponseAsync(It.IsAny<HttpResponseModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StubResponseModel());

        const string tenant = "tenant1";

        // Act
        var result = (await generator.GenerateStubsAsync(input, true, tenant, "prefix", CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.IsTrue(result.All(s => s.Stub.Tenant == tenant));
        Assert.AreEqual("GET request to path /path1", result[0].Stub.Description);

        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.AreEqual("prefixgenerated-e3d8cf9a6a2921845d6720cdcfdb677e", result[0].Stub.Id);
        Assert.AreEqual("prefixgenerated-e3d8cf9a6a2921845d6720cdcfdb677e", result[1].Stub.Id);
        Assert.AreEqual("prefixgenerated-e3d8cf9a6a2921845d6720cdcfdb677e", result[2].Stub.Id);
    }
}
