using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Tests.Integration.RestApi;

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
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = "otherTenant"
        });
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-456",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = tenant
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var reader = new StringReader(content);
        var deserializer = new Deserializer();
        var stubs = deserializer.Deserialize<FullStubDto[]>(reader);
        Assert.AreEqual(1, stubs.Length);
        Assert.AreEqual("test-456", stubs.Single().Stub.Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Tenant_GetAll_Json()
    {
        // arrange
        const string tenant = "tenant1";
        var url = $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs";
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = "otherTenant"
        });
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-456",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = tenant
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypes.JsonMime));

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var stubs = JsonConvert.DeserializeObject<FullStubDto[]>(content);
        Assert.AreEqual(1, stubs.Length);
        Assert.AreEqual("test-456", stubs.Single().Stub.Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Tenant_DeleteAll_HappyFlow()
    {
        // arrange
        const string tenant = "tenant1";
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = "otherTenant"
        });
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-456",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = tenant
        });

        // Act
        using var response = await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs");

        // Assert
        Assert.AreEqual(1, StubSource.GetCollection(null).StubModels.Count);
        Assert.AreEqual("test-123", StubSource.GetCollection(null).StubModels.Single().Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Tenant_UpdateAll_HappyFlow()
    {
        // Arrange
        const string tenant = "tenant1";
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = tenant
        });
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-456",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = tenant
        });

        var stubs = new[]
        {
            new StubDto
            {
                Id = "test-123",
                Conditions = new StubConditionsDto { Method = "GET" },
                Response = new StubResponseDto { Text = "OK" }
            },
            new StubDto
            {
                Id = "test-789",
                Conditions = new StubConditionsDto { Method = "POST" },
                Response = new StubResponseDto { Text = "OK" }
            }
        };

        // Act
        var request =
            new HttpRequestMessage(HttpMethod.Put, $"{TestServer.BaseAddress}ph-api/tenants/{tenant}/stubs")
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(stubs),
                    Encoding.UTF8,
                    MimeTypes.JsonMime)
            };
        using var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(2, StubSource.GetCollection(null).StubModels.Count);
        Assert.AreEqual("test-123", StubSource.GetCollection(null).StubModels[0].Id);
        Assert.AreEqual("test-789", StubSource.GetCollection(null).StubModels[1].Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Tenant_GetTenantNames()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/tenants";
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = "otherTenant"
        });
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-456",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel(),
            Tenant = "tenant1"
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypes.JsonMime));

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
