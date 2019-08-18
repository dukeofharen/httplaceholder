using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Client;
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
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource._stubModels.Add(new StubModel
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
                var stubs = deserializer.Deserialize<IEnumerable<Dto.Stubs.FullStubDto>>(reader);
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
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource._stubModels.Add(new StubModel
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
                var stubs = JsonConvert.DeserializeObject<IEnumerable<Dto.Stubs.FullStubDto>>(content);
                Assert.AreEqual(1, stubs.Count());
                Assert.AreEqual("test-456", stubs.Single().Stub.Id);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_Client()
        {
            // Arrange
            string tenant = "tenant1";
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            // Act
            var result = await GetFactory()
                .TenantClient
                .GetAllAsync(tenant);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("test-456", result.Single().Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_CredentialsAreNeededButIncorrect_ShouldReturn401()
        {
            // Arrange
            string tenant = "tenant1";
            string url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException<ProblemDetails>>(() => GetFactory("wrong", "wrong")
                .TenantClient
                .GetAllAsync(tenant));

            // Assert
            Assert.AreEqual(401, exception.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_CredentialsAreCorrect_ShouldContinue()
        {
            // Arrange
            string tenant = "tenant1";
            string url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var result = await GetFactory("correct", "correct")
                .TenantClient
                .GetAllAsync(tenant);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_DeleteAll_HappyFlow()
        {
            // arrange
            string tenant = "tenant1";
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            // Act
            await GetFactory()
                .TenantClient
                .DeleteAllAsync(tenant);

            // Assert
            Assert.AreEqual(1, StubSource._stubModels.Count);
            Assert.AreEqual("test-123", StubSource._stubModels.Single().Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_UpdateAll_HappyFlow()
        {
            // Arrange
            string tenant = "tenant1";
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });
            StubSource._stubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            var request = new[]
            {
                new Client.StubDto
                {
                    Id = "test-123",
                    Conditions = new Client.StubConditionsDto
                    {
                        Method = "GET"
                    },
                    Response = new Client.StubResponseDto
                    {
                        Text = "OK"
                    }
                },
                new Client.StubDto
                {
                    Id = "test-789",
                    Conditions = new Client.StubConditionsDto
                    {
                        Method = "POST"
                    },
                    Response = new Client.StubResponseDto
                    {
                        Text = "OK"
                    }
                }
            };

            // Act
            await GetFactory()
                .TenantClient
                .UpdateAllAsync(tenant, request);

            // Assert
            Assert.AreEqual(2, StubSource._stubModels.Count);
            Assert.AreEqual("test-123", StubSource._stubModels[0].Id);
            Assert.AreEqual("test-789", StubSource._stubModels[1].Id);
        }
    }
}
