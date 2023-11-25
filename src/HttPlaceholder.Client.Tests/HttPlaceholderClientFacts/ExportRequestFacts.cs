using System.Net;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class ExportRequestFacts : BaseClientTest
{
    private const string CurlResponse = @"{""requestExportType"":""Curl"",""result"":""curl -X PATCH 'http://localhost:5000/http-method' -H 'Accept: */*' -H 'Host: localhost:5000' -H 'User-Agent: hurl/4.0.0'""}";

    [TestMethod]
    public async Task ExportRequestAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string correlationId = "bec89e6a-9bee-4565-bccb-09f0a3363eee";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/export/requests/{correlationId}")
            .WithQueryString("type", "Curl")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(
                () => client.ExportRequestAsync(correlationId, RequestExportType.Curl));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task ExportRequestAsync_ShouldReturnRequest()
    {
        // Arrange
        const string correlationId = "bec89e6a-9bee-4565-bccb-09f0a3363eee";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/export/requests/{correlationId}")
            .WithQueryString("type", "Curl")
            .Respond("application/json", CurlResponse)));

        // Act
        var result = await client.ExportRequestAsync(correlationId, RequestExportType.Curl);

        // Assert
        Assert.AreEqual(RequestExportType.Curl, result.RequestExportType);
        Assert.AreEqual("curl -X PATCH 'http://localhost:5000/http-method' -H 'Accept: */*' -H 'Host: localhost:5000' -H 'User-Agent: hurl/4.0.0'", result.Result);
    }
}
