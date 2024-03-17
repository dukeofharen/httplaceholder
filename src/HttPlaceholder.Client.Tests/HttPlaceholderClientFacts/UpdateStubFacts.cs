using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.StubBuilders;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class UpdateStubFacts : BaseClientTest
{
    [TestMethod]
    public async Task UpdateStubAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string stubId = "stub-id";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Put, $"{BaseUrl}ph-api/stubs/{stubId}")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.UpdateStubAsync(new StubDto(), stubId));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task UpdateStubAsync_ShouldUpdateStub()
    {
        // Arrange
        const string stubId = "stub-id";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Put, $"{BaseUrl}ph-api/stubs/{stubId}")
            .WithPartialContent("test-situation")
            .WithPartialContent("GET")
            .WithPartialContent("OK my dude!")
            .Respond(HttpStatusCode.NoContent)));

        var input = new StubDto
        {
            Id = "test-situation",
            Tenant = "01-get",
            Conditions = new StubConditionsDto
            {
                Method = "GET",
                Url = new StubUrlConditionDto
                {
                    Path = "/testtesttest", Query = new Dictionary<string, object> { { "id", "13" } }
                }
            },
            Response = new StubResponseDto { StatusCode = 200, Text = "OK my dude!" }
        };

        // Act / Assert
        await client.UpdateStubAsync(input, "stub-id");
    }

    [TestMethod]
    public async Task UpdateStubAsync_Builder_ShouldUpdateStub()
    {
        // Arrange
        const string stubId = "stub-id";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Put, $"{BaseUrl}ph-api/stubs/{stubId}")
            .WithPartialContent("OK my dude!")
            .Respond(HttpStatusCode.NoContent)));

        var input = StubBuilder.Begin()
            .WithResponse(StubResponseBuilder.Begin().WithTextResponseBody("OK my dude!"));

        // Act / Assert
        await client.UpdateStubAsync(input, "stub-id");
    }
}
