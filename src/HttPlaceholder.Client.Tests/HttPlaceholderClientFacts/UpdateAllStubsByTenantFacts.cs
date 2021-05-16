using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.StubBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts
{
    [TestClass]
    public class UpdateAllStubsByTenantFacts : BaseClientTest
    {
        [TestMethod]
        public async Task UpdateAllStubsByTenantAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            const string tenant = "01-get";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Put, $"{BaseUrl}ph-api/tenants/{tenant}/stubs")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.UpdateAllStubsByTenantAsync(tenant, Array.Empty<StubDto>()));

            // Assert
            Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task UpdateAllStubsByTenantAsync_ShouldUpdateStubs()
        {
            // Arrange
            const string tenant = "01-get";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Put, $"{BaseUrl}ph-api/tenants/{tenant}/stubs")
                .WithPartialContent("stub1")
                .WithPartialContent("stub2")
                .Respond(HttpStatusCode.NoContent)));

            var input = new[] {new StubDto {Id = "stub1"}, new StubDto {Id = "stub2"}};

            // Act / Assert
            await client.UpdateAllStubsByTenantAsync(tenant, input);
        }

        [TestMethod]
        public async Task UpdateAllStubsByTenantAsync_Builder_ShouldUpdateStubs()
        {
            // Arrange
            const string tenant = "01-get";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Put, $"{BaseUrl}ph-api/tenants/{tenant}/stubs")
                .WithPartialContent("stub1")
                .WithPartialContent("stub2")
                .Respond(HttpStatusCode.NoContent)));

            var stub1 = StubBuilder.Begin().WithId("stub1");
            var stub2 = StubBuilder.Begin().WithId("stub2");
            var input = new[] {stub1, stub2};

            // Act / Assert
            await client.UpdateAllStubsByTenantAsync(tenant, input);
        }
    }
}
