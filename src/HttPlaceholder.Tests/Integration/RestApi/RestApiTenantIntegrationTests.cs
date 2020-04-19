using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HttPlaceholder.Client;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using FullStubDto = HttPlaceholder.Dto.Stubs.FullStubDto;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiTenantIntegrationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_Yaml()
        {
            // arrange
            const string tenant = "tenant1";
            var url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource.StubModels.Add(new StubModel
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
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var reader = new StringReader(content);
            var deserializer = new Deserializer();
            var stubs = deserializer.Deserialize<IEnumerable<FullStubDto>>(reader);
            Assert.AreEqual(1, stubs.Count());
            Assert.AreEqual("test-456", stubs.Single().Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_Json()
        {
            // arrange
            const string tenant = "tenant1";
            var url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource.StubModels.Add(new StubModel
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
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var stubs = JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
            Assert.AreEqual(1, stubs.Count());
            Assert.AreEqual("test-456", stubs.Single().Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_Client()
        {
            // Arrange
            const string tenant = "tenant1";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource.StubModels.Add(new StubModel
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
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("test-456", result.Single().Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetAll_CredentialsAreNeededButIncorrect_ShouldReturn401()
        {
            // Arrange
            const string tenant = "tenant1";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource.StubModels.Add(new StubModel
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
            const string tenant = "tenant1";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource.StubModels.Add(new StubModel
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
            const string tenant = "tenant1";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource.StubModels.Add(new StubModel
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
            Assert.AreEqual(1, StubSource.StubModels.Count);
            Assert.AreEqual("test-123", StubSource.StubModels.Single().Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_UpdateAll_HappyFlow()
        {
            // Arrange
            const string tenant = "tenant1";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = tenant
            });

            var request = new[]
            {
                new StubDto
                {
                    Id = "test-123",
                    Conditions = new StubConditionsDto
                    {
                        Method = "GET"
                    },
                    Response = new StubResponseDto
                    {
                        Text = "OK"
                    }
                },
                new StubDto
                {
                    Id = "test-789",
                    Conditions = new StubConditionsDto
                    {
                        Method = "POST"
                    },
                    Response = new StubResponseDto
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
            Assert.AreEqual(2, StubSource.StubModels.Count);
            Assert.AreEqual("test-123", StubSource.StubModels[0].Id);
            Assert.AreEqual("test-789", StubSource.StubModels[1].Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Tenant_GetTenantNames()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/tenants";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "otherTenant"
            });
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel(),
                Tenant = "tenant1"
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var tenantNames = JsonConvert.DeserializeObject<List<string>>(content);
            Assert.AreEqual(2, tenantNames.Count);
            Assert.AreEqual("otherTenant", tenantNames[0]);
            Assert.AreEqual("tenant1", tenantNames[1]);
        }
    }
}
