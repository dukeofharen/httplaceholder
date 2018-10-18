using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Tests.Integration
{
    [TestClass]
    public class RestApiTenantIntegrationTest : RestApiIntegrationTestBase
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
        public async Task RestApiIntegration_Tenant_GetAll_Yaml()
        {
            // arrange
            string tenant = "tenant1";
            string url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                string content = await response.Content.ReadAsStringAsync();
                var reader = new StringReader(content);
                var deserializer = new Deserializer();
                var stubs = deserializer.Deserialize<IEnumerable<StubModel>>(reader);
                Assert.AreEqual(1, stubs.Count());
                Assert.AreEqual("test-456", stubs.Single().Id);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_Json()
        {
            // arrange
            string tenant = "tenant1";
            string url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                string content = await response.Content.ReadAsStringAsync();
                var stubs = JsonConvert.DeserializeObject<IEnumerable<StubModel>>(content);
                Assert.AreEqual(1, stubs.Count());
                Assert.AreEqual("test-456", stubs.Single().Id);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_CredentialsAreNeededButIncorrect_ShouldReturn401()
        {
            // arrange
            string tenant = "tenant1";
            string url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));
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
        public async Task RestApiIntegration_Tenant_GetAll_CredentialsAreCorrect_ShouldContinue()
        {
            // arrange
            string tenant = "tenant1";
            string url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("correct:correct"))}");

            _config.Add(Constants.ConfigKeys.ApiUsernameKey, "correct");
            _config.Add(Constants.ConfigKeys.ApiPasswordKey, "correct");

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
