using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi.OpenApiImport;

[TestClass]
public class RestApiImportOpenApiPetstoreTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Import_ImportCurl_HappyFlow()
    {
        // Arrange
        var content = await File.ReadAllTextAsync("Resources/httplaceholder.json");

        // Post OpenAPI string to API.
        var url = $"{BaseAddress}ph-api/import/openapi?doNotCreateStub=false";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, Constants.TextMime)
        };
        var curlResponse = await Client.SendAsync(apiRequest);
        curlResponse.EnsureSuccessStatusCode();

        // Get and check the stubs.
        var stubs = StubSource.StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(73, stubs.Length);

        var stub = stubs[20];
        Assert.IsTrue(stub.Id.StartsWith("generated-"));
        Assert.AreEqual("An endpoint which accepts the correlation ID of a request made earlier.\nHttPlaceholder will create a stub based on this request for you to tweak later on.", stub.Description);
        Assert.AreEqual("POST", stub.Conditions.Method);

        var pathRegex = new Regex(@"^\/requests\/(.*)\/stubs$", RegexOptions.Compiled);
        Assert.IsTrue(pathRegex.IsMatch(stub.Conditions.Url.Path));
        Assert.AreEqual(Constants.JsonMime, stub.Conditions.Headers["content-type"]);
        Assert.AreEqual("localhost", stub.Conditions.Host);

        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual(Constants.JsonMime, stub.Response.ContentType);
        Assert.AreEqual(0, stub.Response.Headers.Count);

        var responseBody = JsonConvert.DeserializeObject<Dictionary<string, object>>(stub.Response.Json);
        Assert.IsNotNull(responseBody);
        Assert.IsTrue(responseBody.ContainsKey("stub"));
    }
}
