using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi.OpenApiImport;

[TestClass]
public class RestApiImportOpenApiPetStoreTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Import_ImportOpenApi_HappyFlow()
    {
        // Arrange
        var content = await File.ReadAllTextAsync("Resources/openapi/3.0/petstore.yaml");

        // Post OpenAPI string to API.
        var url = $"{BaseAddress}ph-api/import/openapi?doNotCreateStub=false&tenant=tenant1&stubIdPrefix=prefix-";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, MimeTypes.TextMime)
        };
        var response = await Client.SendAsync(apiRequest);
        response.EnsureSuccessStatusCode();

        // Get and check the stubs.
        var stubs = StubSource.StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(6, stubs.Length);

        var stub = stubs[0];
        Assert.IsTrue(stub.Id.StartsWith("prefix-generated-"));
        Assert.AreEqual("List all pets", stub.Description);

        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("tenant1", stub.Tenant);
        Assert.AreEqual("/v1/pets", ((StubConditionStringCheckingModel)stub.Conditions.Url.Path).StringEquals);
        Assert.AreEqual("42", ((StubConditionStringCheckingModel)stub.Conditions.Url.Query["limit"]).StringEquals);
        Assert.AreEqual("petstore.swagger.io", ((StubConditionStringCheckingModel)stub.Conditions.Host).StringEquals);

        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual(MimeTypes.JsonMime, stub.Response.ContentType);
        Assert.AreEqual(1, stub.Response.Headers.Count);
        Assert.AreEqual("http://petstore.swagger.io/v1/pets/42", stub.Response.Headers["x-next"]);

        var responseBody = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(stub.Response.Json);
        Assert.IsNotNull(responseBody);
        Assert.AreEqual(2, responseBody.Length);
        Assert.IsTrue(responseBody[0].ContainsKey("id"));
        Assert.IsTrue(responseBody[0].ContainsKey("name"));
        Assert.AreEqual(1, (long)responseBody[0]["id"]);
        Assert.AreEqual("Cat", (string)responseBody[0]["name"]);
    }
}
