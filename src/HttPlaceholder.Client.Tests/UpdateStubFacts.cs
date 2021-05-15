using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests
{
    [TestClass]
    public class UpdateStubFacts : BaseClientTest
    {
        [TestMethod]
        public async Task UpdateStubAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            var stubId = "stub-id";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Put, $"{BaseUrl}ph-api/stubs/{stubId}")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.UpdateStubAsync(new StubDto(), stubId));

            // Assert
            Assert.AreEqual($"Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task UpdateStubAsync_ShouldUpdateStub()
        {
            // Arrange
            var stubId = "stub-id";
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
                        Path = "/testtesttest", Query = new Dictionary<string, string> {{"id", "13"}}
                    }
                },
                Response = new StubResponseDto {StatusCode = 200, Text = "OK my dude!"}
            };

            // Act / Assert
            await client.UpdateStubAsync(input, "stub-id");
        }
    }
}
