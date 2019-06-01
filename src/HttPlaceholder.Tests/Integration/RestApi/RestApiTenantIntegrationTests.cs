using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using HttPlaceholder.Dto.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiTenantIntegrationTests : RestApiIntegrationTestBase
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
                var stubs = deserializer.Deserialize<IEnumerable<FullStubDto>>(reader);
                Assert.AreEqual(1, stubs.Count());
                Assert.AreEqual("test-456", stubs.Single().Stub.Id);
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
                var stubs = JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
                Assert.AreEqual(1, stubs.Count());
                Assert.AreEqual("test-456", stubs.Single().Stub.Id);
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

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

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

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_DeleteAll_HappyFlow()
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
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
                Assert.AreEqual(1, _stubSource._stubModels.Count);
                Assert.AreEqual("test-123", _stubSource._stubModels.Single().Id);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_UpdateAll_HappyFlow()
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
                Tenant = tenant
            });
            _stubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            string body = @"
- id: test-123
  conditions:
    method: GET
  response:
    text: OK
- id: test-789
  conditions:
    method: POST
  response:
    text: OK
";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "text/yaml")
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
                Assert.AreEqual(2, _stubSource._stubModels.Count);
                Assert.AreEqual("test-123", _stubSource._stubModels[0].Id);
                Assert.AreEqual("test-789", _stubSource._stubModels[1].Id);
            }
        }
    }
}
