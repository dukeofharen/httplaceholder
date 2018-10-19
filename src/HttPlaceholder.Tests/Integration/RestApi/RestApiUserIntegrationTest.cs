using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiUserIntegrationTest : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            InitializeRestApiIntegrationTest();
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanupRestApiIntegrationTest();
        }

        [TestMethod]
        public async Task RestApiIntegration_User_Get_CredentialsAreIncorrect_ShouldReturn401()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/users/user";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("wrong:wrong"))}");

            _config.Add(Constants.ConfigKeys.ApiUsernameKey, "correct");
            _config.Add(Constants.ConfigKeys.ApiPasswordKey, "correct");

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_User_Get_CredentialsAreCorrect_UsernameDoesntMatch_ShouldReturn403()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/users/wrong";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("correct:correct"))}");

            _config.Add(Constants.ConfigKeys.ApiUsernameKey, "correct");
            _config.Add(Constants.ConfigKeys.ApiPasswordKey, "correct");

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_User_Get_CredentialsAreCorrect_UsernameMatches_ShouldReturn200()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/users/correct";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("correct:correct"))}");

            _config.Add(Constants.ConfigKeys.ApiUsernameKey, "correct");
            _config.Add(Constants.ConfigKeys.ApiPasswordKey, "correct");

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                string content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UserModel>(content);
                Assert.AreEqual("correct", model.Username);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_User_Get_NoAuthenticationConfigured_ShouldReturn200()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/users/correct";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                string content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UserModel>(content);
                Assert.AreEqual("correct", model.Username);
            }
        }
    }
}
