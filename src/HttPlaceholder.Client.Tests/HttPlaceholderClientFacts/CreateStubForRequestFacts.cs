using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class CreateStubForRequestFacts : BaseClientTest
{
    private const string CreateStubResult = @"{
    ""stub"": {
        ""id"": ""generated-be3ac75bbea159e0ca1224336ce23aff"",
        ""conditions"": {
            ""method"": ""GET"",
            ""url"": {
                ""path"": ""/test.html"",
                ""query"": {},
                ""isHttps"": false
            },
            ""headers"": {
                ""Connection"": ""keep-alive"",
                ""Accept"": ""text/html,application/xhtml\\+xml,application/xml;q=0\\.9,image/webp,\\*/\\*;q=0\\.8"",
                ""Accept-Encoding"": ""gzip,\\ deflate"",
                ""Accept-Language"": ""en-US,en;q=0\\.5"",
                ""User-Agent"": ""Mozilla/5\\.0\\ \\(X11;\\ Ubuntu;\\ Linux\\ x86_64;\\ rv:88\\.0\\)\\ Gecko/20100101\\ Firefox/88\\.0"",
                ""Upgrade-Insecure-Requests"": ""1""
            },
            ""clientIp"": ""127.0.0.1"",
            ""host"": ""localhost:5000""
        },
        ""response"": {
            ""text"": ""OK!"",
            ""headers"": {}
        },
        ""priority"": 0,
        ""enabled"": true
    },
    ""metadata"": {
        ""readOnly"": false
    }
}";

    [TestMethod]
    public async Task CreateStubForRequestAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string correlationId = "95890e55-0be2-4c40-9046-7c7b291693ce";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/requests/{correlationId}/stubs")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.CreateStubForRequestAsync(correlationId));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task CreateStubForRequestAsync_ShouldCreateStub_NoInput()
    {
        // Arrange
        const string correlationId = "95890e55-0be2-4c40-9046-7c7b291693ce";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/requests/{correlationId}/stubs")
            .WithContent("{}")
            .Respond("application/json", CreateStubResult)));

        // Act
        var result = await client.CreateStubForRequestAsync(correlationId);

        // Assert
        Assert.IsNotNull(result.Stub);
        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual("generated-be3ac75bbea159e0ca1224336ce23aff", result.Stub.Id);
        Assert.AreEqual("GET", result.Stub.Conditions.Method);
    }

    [TestMethod]
    public async Task CreateStubForRequestAsync_ShouldCreateStub_WithInput()
    {
        // Arrange
        const string correlationId = "95890e55-0be2-4c40-9046-7c7b291693ce";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/requests/{correlationId}/stubs")
            .WithContent(@"{""DoNotCreateStub"":true}")
            .Respond("application/json", CreateStubResult)));

        // Act
        var result = await client.CreateStubForRequestAsync(correlationId,
            new CreateStubForRequestInputDto {DoNotCreateStub = true});

        // Assert
        Assert.IsNotNull(result.Stub);
        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual("generated-be3ac75bbea159e0ca1224336ce23aff", result.Stub.Id);
        Assert.AreEqual("GET", result.Stub.Conditions.Method);
    }
}
