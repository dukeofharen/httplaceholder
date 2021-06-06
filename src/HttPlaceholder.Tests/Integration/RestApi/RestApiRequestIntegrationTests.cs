using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using HttPlaceholder.Dto.v1.Requests;
using HttPlaceholder.TestUtilities.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiRequestIntegrationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_Request_GetAll()
        {
            // Arrange
            var correlation = Guid.NewGuid().ToString();
            StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

            // Act
            using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RequestResultDto[]>(content);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(correlation, result.First().CorrelationId);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_GetSingle_RequestNotFound_ShouldReturn404()
        {
            // Arrange
            var correlation = Guid.NewGuid().ToString();
            StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

            // Act
            using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}1");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_GetSingle_RequestFound_ShouldReturnRequest()
        {
            // Arrange
            var correlation = Guid.NewGuid().ToString();
            StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

            // Act
            using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RequestResultDto>(content);

            // Assert
            Assert.AreEqual(correlation, result.CorrelationId);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_GetOverview()
        {
            // Arrange
            var correlation = Guid.NewGuid().ToString();
            StubSource.RequestResultModels.Add(new RequestResultModel {CorrelationId = correlation});

            // Act
            using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/overview");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RequestOverviewDto[]>(content);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(correlation, result.First().CorrelationId);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_GetByStubId()
        {
            // Arrange
            StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub2"});
            StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub1"});

            // Act
            using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/stubs/stub1/requests");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RequestResultDto[]>(content);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("stub1", result.First().ExecutingStubId);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_DeleteAllRequests()
        {
            // Perform a few requests.
            StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub2"});
            StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub1"});

            // Act
            using var response = await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/requests");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(0, StubSource.RequestResultModels.Count);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_DeleteRequest()
        {
            // Perform a few requests.
            var request1 =
                new RequestResultModel {ExecutingStubId = "stub1", CorrelationId = Guid.NewGuid().ToString()};
            var request2 =
                new RequestResultModel {ExecutingStubId = "stub2", CorrelationId = Guid.NewGuid().ToString()};
            StubSource.RequestResultModels.Add(request1);
            StubSource.RequestResultModels.Add(request2);

            // Act
            using var response = await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/requests/{request2.CorrelationId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, StubSource.RequestResultModels.Count);
            Assert.IsTrue(StubSource.RequestResultModels.Any(r => r.CorrelationId == request1.CorrelationId));
        }
    }
}
