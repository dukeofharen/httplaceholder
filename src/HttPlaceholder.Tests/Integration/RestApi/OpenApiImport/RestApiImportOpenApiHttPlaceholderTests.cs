using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi.OpenApiImport;

[TestClass]
public class RestApiImportOpenApiHttPlaceholderTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Import_ImportOpenApi_HappyFlow()
    {
        // Arrange
        var content = await File.ReadAllTextAsync("Resources/openapi/3.0/httplaceholder.json");

        // Post OpenAPI string to API.
        var url = $"{BaseAddress}ph-api/import/openapi?doNotCreateStub=false&tenant=tenant1";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, MimeTypes.TextMime)
        };
        var response = await Client.SendAsync(apiRequest);
        response.EnsureSuccessStatusCode();

        // Get and check the stubs.
        var stubs = StubSource.GetCollection(null).StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(73, stubs.Length);

        var stub = stubs[20];
        Assert.IsTrue(stub.Id.StartsWith("generated-"));
        Assert.AreEqual(
            "An endpoint which accepts the correlation ID of a request made earlier.\nHttPlaceholder will create a stub based on this request for you to tweak later on.",
            stub.Description);
        Assert.AreEqual("POST", stub.Conditions.Method);
        Assert.AreEqual("tenant1", stub.Tenant);

        var pathRegex = new Regex(@"^\/requests\/(.*)\/stubs$", RegexOptions.Compiled);
        Assert.IsTrue(pathRegex.IsMatch(((StubConditionStringCheckingModel)stub.Conditions.Url.Path).StringEquals));
        Assert.AreEqual(MimeTypes.JsonMime,
            ((StubConditionStringCheckingModel)stub.Conditions.Headers[HeaderKeys.ContentType]).StringEquals);
        Assert.AreEqual("localhost", ((StubConditionStringCheckingModel)stub.Conditions.Host).StringEquals);

        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual(MimeTypes.JsonMime, stub.Response.ContentType);
        Assert.AreEqual(0, stub.Response.Headers.Count);

        var responseBody = JsonConvert.DeserializeObject<Dictionary<string, object>>(stub.Response.Json);
        Assert.IsNotNull(responseBody);
        Assert.IsTrue(responseBody.ContainsKey("stub"));
    }
}
