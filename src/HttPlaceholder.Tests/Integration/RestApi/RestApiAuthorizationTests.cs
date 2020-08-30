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
        private static (string relativeUrl, HttpMethod httpMethod, bool expect401, bool postArray)[] _urls = new[]
        {
            ("ph-api/metadata", HttpMethod.Get, false, false), ("ph-api/requests", HttpMethod.Get, true, false),
            ("ph-api/requests/overview", HttpMethod.Get, true, false),
            ("ph-api/requests", HttpMethod.Delete, true, false),
            ("ph-api/stubs/stub-123/requests", HttpMethod.Get, true, false),
            ("ph-api/requests/babceb20-d386-4741-8006-67cbccf33810/stubs", HttpMethod.Post, true, false),
            ("ph-api/stubs", HttpMethod.Post, true, false), ("ph-api/stubs", HttpMethod.Get, true, false),
            ("ph-api/stubs", HttpMethod.Delete, true, false),
            ("ph-api/stubs/stub-123", HttpMethod.Put, true, false),
            ("ph-api/stubs/stub-123", HttpMethod.Get, true, false),
            ("ph-api/stubs/stub-123", HttpMethod.Delete, true, false),
            ("ph-api/tenants", HttpMethod.Get, true, false),
            ("ph-api/tenants/tenantname/stubs", HttpMethod.Get, true, false),
            ("ph-api/tenants/tenantname/stubs", HttpMethod.Delete, true, false),
            ("ph-api/tenants/tenantname/stubs", HttpMethod.Put, true, true),
            ("ph-api/users/username", HttpMethod.Put, false, false)
        };

        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_IncorrectCredentials_ShouldReturn401()
        {
            // Arrange
            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            foreach (var url in _urls)
            {
                // Act
                var request =
                    new HttpRequestMessage(url.httpMethod, $"{TestServer.BaseAddress}{url.relativeUrl}");
                request.Headers.Add("Authorization", HttpUtilities.GetBasicAuthHeaderValue("wrong", "wrong"));
                if (url.httpMethod == HttpMethod.Post || url.httpMethod == HttpMethod.Put)
                {
                    request.Content = new StringContent(url.postArray ? "[]" : "{}", Encoding.UTF8, "application/json");
                }

                using var response = await Client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                // Assert
                if (url.expect401)
                {
                    Assert.AreEqual(
                        HttpStatusCode.Unauthorized,
                        response.StatusCode,
                        $"Expected call to relative URL {url.relativeUrl} ({url.httpMethod}) to return HTTP status code 401 but got {(int)response.StatusCode} with returned content '{content}'.");
                }
                else
                {
                    Assert.IsFalse(
                        response.StatusCode != HttpStatusCode.Unauthorized && (int)response.StatusCode >= 500,
                        $"Expected call to relative URL {url.relativeUrl} ({url.httpMethod}) to NOT return 401 and no 5xx error, but returned {(int)response.StatusCode} with returned content '{content}'.");
                }
            }
        }
    }
}
