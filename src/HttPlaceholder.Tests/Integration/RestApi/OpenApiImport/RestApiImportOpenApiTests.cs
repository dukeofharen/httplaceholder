using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.RestApi.OpenApiImport;

[TestClass]
public class RestApiImportOpenApiTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [DataTestMethod]
    // [DataRow("Resources/openapi/2.0/api-with-examples.json", 4)]
    // [DataRow("Resources/openapi/2.0/api-with-examples.yaml", 4)]
    // [DataRow("Resources/openapi/2.0/petstore.json", 6)]
    // [DataRow("Resources/openapi/2.0/petstore.yaml", 6)]
    // [DataRow("Resources/openapi/2.0/petstore-expanded.json", 8)]
    // [DataRow("Resources/openapi/2.0/petstore-expanded.yaml", 8)]
    // [DataRow("Resources/openapi/2.0/petstore-minimal.json", 1)]
    // [DataRow("Resources/openapi/2.0/petstore-minimal.yaml", 1)]
    // [DataRow("Resources/openapi/2.0/petstore-simple.json", 8)]
    // [DataRow("Resources/openapi/2.0/petstore-simple.yaml", 8)]
    // [DataRow("Resources/openapi/2.0/petstore-with-external-docs.json", 8)]
    // [DataRow("Resources/openapi/2.0/petstore-with-external-docs.yaml", 8)]
    // [DataRow("Resources/openapi/2.0/uber.yaml", 10)]
    // [DataRow("Resources/openapi/2.0/uber.json", 10)]
    // [DataRow("Resources/openapi/3.0/petstore.yaml", 6)]
    // [DataRow("Resources/openapi/3.0/httplaceholder.json", 73)]
    // [DataRow("Resources/openapi/3.0/openapi3_example.yaml", 4)]
    // [DataRow("Resources/openapi/3.0/minimal_example.yaml", 1)]
    // [DataRow("Resources/openapi/3.0/petstore_plain.yaml", 6)]
    // [DataRow("Resources/openapi/3.0/api-with-examples.json", 4)]
    // [DataRow("Resources/openapi/3.0/api-with-examples.yaml", 4)]
    // [DataRow("Resources/openapi/3.0/callback-example.json", 1)]
    // [DataRow("Resources/openapi/3.0/callback-example.yaml", 1)]
    // [DataRow("Resources/openapi/3.0/link-example.json", 6)]
    // [DataRow("Resources/openapi/3.0/link-example.yaml", 6)]
    // [DataRow("Resources/openapi/3.0/petstore-expanded.json", 8)]
    // [DataRow("Resources/openapi/3.0/petstore-expanded.yaml", 8)]
    [DataRow("Resources/openapi/3.0/uspto.json", 6)]
    // [DataRow("Resources/openapi/3.0/uspto.yaml", 6)]
    public async Task RestApiIntegration_Import_ImportOpenApi_Regression(string definitionPath,
        int expectedNumberOfStubs)
    {
        // Arrange
        var content = await File.ReadAllTextAsync(definitionPath);

        // Post OpenAPI string to API.
        var url = $"{BaseAddress}ph-api/import/openapi?doNotCreateStub=false&tenant=tenant1";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, Constants.TextMime)
        };
        var response = await Client.SendAsync(apiRequest);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail($"HTTP status '{response.StatusCode}' returned with content '{responseContent}'.");
        }

        // Get and check the stubs.
        var stubs = StubSource.StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(expectedNumberOfStubs, stubs.Length);
    }
}
