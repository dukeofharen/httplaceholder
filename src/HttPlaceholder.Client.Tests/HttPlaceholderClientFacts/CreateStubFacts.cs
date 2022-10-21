using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.StubBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class CreateStubFacts : BaseClientTest
{
    private const string CreateStubResponse = @"{
    ""stub"": {
        ""id"": ""test-situation"",
        ""conditions"": {
            ""method"": ""GET"",
            ""url"": {
                ""path"": ""/testtesttest"",
                ""query"": {
                    ""id"": ""13""
                }
            }
        },
        ""response"": {
            ""statusCode"": 200,
            ""text"": ""OK my dude!""
        },
        ""priority"": 0,
        ""tenant"": ""01-get"",
        ""enabled"": true
    },
    ""metadata"": {
        ""readOnly"": false
    }
}";

    [TestMethod]
    public async Task CreateStubAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/stubs")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(
                () => client.CreateStubAsync(new StubDto()));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task CreateStubAsync_ShouldCreateStub()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/stubs")
            .WithPartialContent("test-situation")
            .WithPartialContent("GET")
            .WithPartialContent("OK my dude!")
            .Respond("application/json", CreateStubResponse)));
        var input = new StubDto
        {
            Id = "test-situation",
            Tenant = "01-get",
            Conditions = new StubConditionsDto
            {
                Method = "GET",
                Url = new StubUrlConditionDto
                {
                    Path = "/testtesttest", Query = new Dictionary<string, object> {{"id", "13"}}
                }
            },
            Response = new StubResponseDto {StatusCode = 200, Text = "OK my dude!"}
        };

        // Act
        var result = await client.CreateStubAsync(input);

        // Assert
        Assert.IsNotNull(result.Stub);
        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual("test-situation", result.Stub.Id);
        Assert.AreEqual("GET", result.Stub.Conditions.Method);
    }

    [TestMethod]
    public async Task CreateStubAsync_Builder_ShouldCreateStub()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/stubs")
            .WithPartialContent("stub123")
            .Respond("application/json", CreateStubResponse)));
        var input = StubBuilder.Begin().WithId("stub123");

        // Act
        var result = await client.CreateStubAsync(input);

        // Assert
        Assert.IsNotNull(result.Stub);
        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual("test-situation", result.Stub.Id);
        Assert.AreEqual("GET", result.Stub.Conditions.Method);
    }
}
