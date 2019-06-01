using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiRequestIntegrationTests : RestApiIntegrationTestBase
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
        public async Task RestApiIntegration_Request_GetAll()
        {
            // arrange
            string correlation = Guid.NewGuid().ToString();
            string url = $"{TestServer.BaseAddress}ph-api/requests";
            _stubSource._requestResultModels.Add(new RequestResultModel
            {
                CorrelationId = correlation
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                string content = await response.Content.ReadAsStringAsync();
                var requests = JsonConvert.DeserializeObject<IEnumerable<RequestResultModel>>(content);
                Assert.AreEqual(1, requests.Count());
                Assert.AreEqual(correlation, requests.First().CorrelationId);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_GetByStubId()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/requests/stub1";
            _stubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub2"
            });
            _stubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub1"
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                string content = await response.Content.ReadAsStringAsync();
                var requests = JsonConvert.DeserializeObject<IEnumerable<RequestResultModel>>(content);
                Assert.AreEqual(1, requests.Count());
                Assert.AreEqual("stub1", requests.First().ExecutingStubId);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_CredentialsAreNeededButIncorrect_ShouldReturn401()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/requests/stub1";
            _stubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub2"
            });
            _stubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub1"
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("wrong:wrong"))}");

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Request_CredentialsAreCorrect_ShouldContinue()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ph-api/requests/stub1";
            _stubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub2"
            });
            _stubSource._requestResultModels.Add(new RequestResultModel
            {
                ExecutingStubId = "stub1"
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("correct:correct"))}");

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
