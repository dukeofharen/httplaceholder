using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Client;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiUserIntegrationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_User_Get_CredentialsAreIncorrect_ShouldReturn401()
        {
            // Arrange
            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException<ProblemDetails>>(() => GetFactory("wrong", "wrong")
            .UserClient
            .GetAsync("wrong"));

            // Assert
            Assert.AreEqual(401, exception.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_User_Get_CredentialsAreCorrect_UsernameDoesntMatch_ShouldReturn403()
        {
            // Arrange
            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException<ProblemDetails>>(() => GetFactory("correct", "correct")
            .UserClient
            .GetAsync("wrong"));

            // Assert
            Assert.AreEqual(403, exception.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_User_Get_CredentialsAreCorrect_UsernameMatches_ShouldReturn200()
        {
            // Arrange
            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var result = await GetFactory("correct", "correct")
            .UserClient
            .GetAsync("correct");

            // Assert
            Assert.AreEqual("correct", result.Username);
        }

        [TestMethod]
        public async Task RestApiIntegration_User_Get_NoAuthenticationConfigured_ShouldReturn200()
        {
            // Act
            var result = await GetFactory()
            .UserClient
            .GetAsync("correct");

            // Assert
            Assert.AreEqual("correct", result.Username);
        }
    }
}
