using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.Export.Implementations;
using HttPlaceholder.Application.StubExecution.Models.HAR;
using HttPlaceholder.Common;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.Tests.Export.Implementations;

[TestClass]
public class RequestToHarServiceFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void Convert_HappyFlow()
    {
        // Arrange
        const string version = "1.2.3.4";

        var assemblyServiceMock = _mocker.GetMock<IAssemblyService>();
        assemblyServiceMock
            .Setup(m => m.GetAssemblyVersion())
            .Returns(version);

        var service = _mocker.CreateInstance<RequestToHarService>();

        var request = new RequestResultModel
        {
            CorrelationId = Guid.NewGuid().ToString(),
            HasResponse = true,
            RequestBeginTime = DateTime.Now,
            RequestParameters = new RequestParametersModel
            {
                Method = "POST",
                Headers = new Dictionary<string, string>
                {
                    { HeaderKeys.ContentType, "application/json" }, { "X-Some-Header", "SomeValue" }
                },
                BinaryBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { key = "val" })),
                Url = "http://localhost:5000/some-url?q1=v1&q2=v2"
            }
        };
        var response = new ResponseModel
        {
            Headers = new Dictionary<string, string>
            {
                { HeaderKeys.ContentType, "application/json" }, { "X-Some-Header", "SomeValue" }
            },
            Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { key = "responsevalue" })),
            BodyIsBinary = false,
            StatusCode = 200
        };

        // Act
        var result = service.Convert(request, response);

        // Assert
        var har = JsonConvert.DeserializeObject<Har>(result);

        var log = har.Log;
        Assert.AreEqual("1.2", log.Version);

        var creator = log.Creator;
        Assert.AreEqual("HttPlaceholder", creator.Name);
        Assert.AreEqual(version, creator.Version);

        var pages = log.Pages;
        Assert.AreEqual(1, pages.Length);
        var page = pages.Single();
        Assert.AreEqual($"page_{request.CorrelationId}", page.Id);
        Assert.AreEqual(request.RequestBeginTime, page.StartedDateTime);

        var entries = log.Entries;
        Assert.AreEqual(1, entries.Length);
        var entry = entries.Single();
        Assert.AreEqual($"page_{request.CorrelationId}", entry.PageRef);
        Assert.AreEqual(request.RequestBeginTime, entry.StartedDateTime);

        var harRequest = entry.Request;
        Assert.AreEqual(harRequest.Method, request.RequestParameters.Method);
        Assert.AreEqual(harRequest.Url, request.RequestParameters.Url);
        Assert.AreEqual(harRequest.HttpVersion, "HTTP/1.1");
        var harRequestHeaders = harRequest.Headers;
        Assert.AreEqual(2, harRequestHeaders.Length);
        Assert.AreEqual("SomeValue", harRequestHeaders.Single(h => h.Name == "X-Some-Header").Value);
        var harPostData = harRequest.PostData;
        Assert.AreEqual("application/json", harPostData.MimeType);
        Assert.AreEqual("{\"key\":\"val\"}", harPostData.Text);
        var harQuery = harRequest.QueryString;
        Assert.AreEqual(2, harQuery.Length);
        Assert.AreEqual("v1", harQuery.Single(q => q.Name == "q1").Value);

        var harResponse = entry.Response;
        Assert.AreEqual(response.StatusCode, harResponse.Status);
        Assert.AreEqual("OK", harResponse.StatusText);
        Assert.AreEqual("HTTP/1.1", harResponse.HttpVersion);
        var harResponseHeaders = harResponse.Headers;
        Assert.AreEqual("SomeValue", harResponseHeaders.Single(h => h.Name == "X-Some-Header").Value);
        var harResponseContent = harResponse.Content;
        Assert.IsNull(harResponseContent.Encoding);
        Assert.AreEqual("application/json", harResponseContent.MimeType);
        Assert.AreEqual(23, harResponseContent.Size);
        Assert.AreEqual("{\"key\":\"responsevalue\"}", harResponseContent.Text);
    }

    [TestMethod]
    public void Convert_HappyFlow_BinaryResponse()
    {
        // Arrange
        const string version = "1.2.3.4";

        var assemblyServiceMock = _mocker.GetMock<IAssemblyService>();
        assemblyServiceMock
            .Setup(m => m.GetAssemblyVersion())
            .Returns(version);

        var service = _mocker.CreateInstance<RequestToHarService>();

        var request = new RequestResultModel
        {
            CorrelationId = Guid.NewGuid().ToString(),
            HasResponse = true,
            RequestBeginTime = DateTime.Now,
            RequestParameters = new RequestParametersModel
            {
                Method = "POST",
                Headers = new Dictionary<string, string> { { HeaderKeys.ContentType, "application/json" } },
                BinaryBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { key = "val" })),
                Url = "http://localhost:5000/some-url?q1=v1&q2=v2"
            }
        };
        var response = new ResponseModel
        {
            Headers = new Dictionary<string, string> { { HeaderKeys.ContentType, "image/png" } },
            Body = new byte[] { 1, 2, 3 },
            BodyIsBinary = true,
            StatusCode = 200
        };

        // Act
        var result = service.Convert(request, response);

        // Assert
        var har = JsonConvert.DeserializeObject<Har>(result);
        var harResponseContent = har.Log.Entries.Single().Response.Content;
        Assert.AreEqual("base64", harResponseContent.Encoding);
        Assert.AreEqual("image/png", harResponseContent.MimeType);
        Assert.AreEqual(3, harResponseContent.Size);
        Assert.AreEqual("AQID", harResponseContent.Text);
    }
}
