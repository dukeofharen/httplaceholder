using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration
{
    [TestClass]
    public class GenericIntegrationTests : IntegrationTestBase
    {
        private InMemoryStubSource _stubSource;

        [TestInitialize]
        public void Initialize()
        {
            _stubSource = new InMemoryStubSource(Options);
            InitializeIntegrationTest(new (Type, object)[] { (typeof(IStubSource), _stubSource) });
        }

        [TestCleanup]
        public void Cleanup() => CleanupIntegrationTest();

        [TestMethod]
        public async Task GenericIntegration_SwaggerUi_IsApproachable()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}swagger/index.html";

            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task GenericIntegration_SwaggerJson_IsApproachable()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}swagger/v1/swagger.json";

            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task GenericIntegration_Ui_Returns404()
        {
            // The URL ph-ui is not executed as stub, so it doesn't return an HTTP 500 when called.

            // arrange
            var url = $"{TestServer.BaseAddress}ph-ui";

            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.AreNotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
