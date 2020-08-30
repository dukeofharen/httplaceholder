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
        public async Task RestApiIntegration_Request_CredentialsAreCorrect_ShouldContinue()
        {
            // Arrange
            StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub2"});
            StubSource.RequestResultModels.Add(new RequestResultModel {ExecutingStubId = "stub1"});

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, $"{TestServer.BaseAddress}ph-api/stubs/stub1/requests");
            request.Headers.Add("Authorization", HttpUtilities.GetBasicAuthHeaderValue("correct", "correct"));
            using var response = await Client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
