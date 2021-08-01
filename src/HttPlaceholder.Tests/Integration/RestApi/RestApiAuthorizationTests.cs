using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.TestUtilities.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiAuthorizationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [DataTestMethod]
        [DataRow("ph-api/metadata", "GET", false, false)]
        [DataRow("ph-api/requests", "GET", true, false)]
        [DataRow("ph-api/requests/3b392b80-b35f-4a4d-be01-e95d2d42d869", "GET", true, false)]
        [DataRow("ph-api/requests/overview", "GET", true, false)]
        [DataRow("ph-api/requests", "DELETE", true, false)]
        [DataRow("ph-api/stubs/stub-123/requests", "GET", true, false)]
        [DataRow("ph-api/requests/babceb20-d386-4741-8006-67cbccf33810/stubs", "POST", true, false)]
        [DataRow("ph-api/stubs", "POST", true, false)]
        [DataRow("ph-api/stubs/multiple", "POST", true, true)]
        [DataRow("ph-api/stubs", "GET", true, false)]
        [DataRow("ph-api/stubs/overview", "GET", true, false)]
        [DataRow("ph-api/stubs", "DELETE", true, false)]
        [DataRow("ph-api/stubs/stub-123", "PUT", true, false)]
        [DataRow("ph-api/stubs/stub-123", "GET", true, false)]
        [DataRow("ph-api/stubs/stub-123", "DELETE", true, false)]
        [DataRow("ph-api/tenants", "GET", true, false)]
        [DataRow("ph-api/tenants/tenantname/stubs", "GET", true, false)]
        [DataRow("ph-api/tenants/tenantname/stubs", "DELETE", true, false)]
        [DataRow("ph-api/tenants/tenantname/stubs", "PUT", true, true)]
        [DataRow("ph-api/users/username", "PUT", false, false)]
        public async Task RestApiIntegration_IncorrectCredentials_ShouldReturn401(
            string relativeUrl,
            string httpMethod,
            bool expect401,
            bool postArray)
        {
            // Arrange
            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var parsedHttpMethod = new HttpMethod(httpMethod);
            var request =
                new HttpRequestMessage(parsedHttpMethod, $"{TestServer.BaseAddress}{relativeUrl}");
            request.Headers.Add("Authorization", HttpUtilities.GetBasicAuthHeaderValue("wrong", "wrong"));
            if (parsedHttpMethod == HttpMethod.Post || parsedHttpMethod == HttpMethod.Put)
            {
                request.Content = new StringContent(postArray ? "[]" : "{}", Encoding.UTF8, "application/json");
            }

            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            if (expect401)
            {
                Assert.AreEqual(
                    HttpStatusCode.Unauthorized,
                    response.StatusCode,
                    $"Expected call to relative URL {relativeUrl} ({httpMethod}) to return HTTP status code 401 but got {(int)response.StatusCode} with returned content '{content}'.");
            }
            else
            {
                Assert.IsFalse(
                    response.StatusCode == HttpStatusCode.Unauthorized || (int)response.StatusCode >= 500,
                    $"Expected call to relative URL {relativeUrl} ({httpMethod}) to NOT return 401 and no 5xx error, but returned {(int)response.StatusCode} with returned content '{content}'.");
            }
        }

        [DataTestMethod]
        [DataRow("ph-api/metadata", "GET")]
        [DataRow("ph-api/requests", "GET")]
        [DataRow("ph-api/requests/3b392b80-b35f-4a4d-be01-e95d2d42d869", "GET")]
        [DataRow("ph-api/requests/overview", "GET")]
        [DataRow("ph-api/requests", "DELETE")]
        [DataRow("ph-api/stubs/stub-123/requests", "GET")]
        [DataRow("ph-api/requests/babceb20-d386-4741-8006-67cbccf33810/stubs", "POST")]
        [DataRow("ph-api/stubs", "POST")]
        [DataRow("ph-api/stubs", "GET")]
        [DataRow("ph-api/stubs/overview", "GET")]
        [DataRow("ph-api/stubs", "DELETE")]
        [DataRow("ph-api/stubs/stub-123", "PUT")]
        [DataRow("ph-api/stubs/stub-123", "GET")]
        [DataRow("ph-api/stubs/stub-123", "DELETE")]
        [DataRow("ph-api/tenants", "GET")]
        [DataRow("ph-api/tenants/tenantname/stubs", "GET")]
        [DataRow("ph-api/tenants/tenantname/stubs", "DELETE")]
        [DataRow("ph-api/tenants/tenantname/stubs", "PUT")]
        [DataRow("ph-api/users/username", "PUT")]
        public async Task RestApiIntegration_CorrectCredentials_ShouldNotReturn401(
            string relativeUrl,
            string httpMethod)
        {
            // Arrange
            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var parsedHttpMethod = new HttpMethod(httpMethod);
            var request =
                new HttpRequestMessage(parsedHttpMethod, $"{TestServer.BaseAddress}{relativeUrl}");
            request.Headers.Add("Authorization", HttpUtilities.GetBasicAuthHeaderValue("correct", "correct"));
            if (parsedHttpMethod == HttpMethod.Post || parsedHttpMethod == HttpMethod.Put)
            {
                request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            }

            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.IsFalse(
                response.StatusCode == HttpStatusCode.Unauthorized || (int)response.StatusCode >= 500,
                $"Expected call to relative URL {relativeUrl} ({httpMethod}) to NOT return 401 and no 5xx error, but returned {(int)response.StatusCode} with returned content '{content}'.");
        }
    }
}
