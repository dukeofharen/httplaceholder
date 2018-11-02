using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiMetadataIntegrationTests : RestApiIntegrationTestBase
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
        public async Task RestApiIntegration_Metadata_Get_CredentialsAreCorrect_UsernameMatches_ShouldReturn200()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/metadata";
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
                var model = JsonConvert.DeserializeObject<MetadataModel>(content);
                Assert.IsFalse(string.IsNullOrWhiteSpace(model.Version));
            }
        }
    }
}
