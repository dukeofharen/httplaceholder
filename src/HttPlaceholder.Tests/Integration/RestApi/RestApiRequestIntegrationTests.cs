using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Client;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            StubSource._requestResultModels.Add(new RequestResultModel
            {
                CorrelationId = correlation
            });

            // Act
            var result = await GetFactory().RequestClient.GetAllAsync();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(correlation, result.First().CorrelationId);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_GetByStubId()
        {
            // Arrange
            StubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub2"
            });
            StubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub1"
            });

            // Act
            var result = await GetFactory().RequestClient.GetByStubIdAsync("stub1");

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("stub1", result.First().ExecutingStubId);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_CredentialsAreNeededButIncorrect_ShouldReturn401()
        {
            // Arrange
            StubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub2"
            });
            StubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub1"
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException<ProblemDetails>>(() => GetFactory("wrong", "wrong").RequestClient.GetByStubIdAsync("stub1"));

            // Assert
            Assert.AreEqual(401, exception.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_CredentialsAreCorrect_ShouldContinue()
        {
            // Arrange
            StubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub2"
            });
            StubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub1"
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var result = await GetFactory("correct", "correct").RequestClient.GetByStubIdAsync("stub1");

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
