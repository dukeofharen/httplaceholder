using System.Linq;
using System.Net;
using System.Net.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.OpenApi.Readers;

namespace HttPlaceholder.Tests.Integration;

[TestClass]
public class GenericIntegrationTests : IntegrationTestBase
{
    private InMemoryStubSource _stubSource;

    [TestInitialize]
    public void Initialize()
    {
        _stubSource = new InMemoryStubSource(Options);
        InitializeIntegrationTest(new (Type, object)[] {(typeof(IStubSource), _stubSource)});
    }

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task GenericIntegration_SwaggerUi_IsApproachable()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}swagger/index.html";

        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // Act / Assert
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task GenericIntegration_SwaggerJson_IsApproachableAndHasCustomPropertiesAdded()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}swagger/v1/swagger.json";

        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // Act
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var openapi = new OpenApiStringReader().Read(content, out _);

        // Assert StubJsonPathDto
        var stubJsonPathDtoSchema = openapi.Components.Schemas["StubJsonPathDto"];
        Assert.AreEqual(2, stubJsonPathDtoSchema.Properties.Count);
        Assert.IsFalse(stubJsonPathDtoSchema.Properties.Any(p => p.Value.Type == "null"));

        // Assert StubConditionStringCheckingDto
        var stubConditionStringCheckingDtoSchema = openapi.Components.Schemas["StubConditionStringCheckingDto"];
        Assert.AreEqual(22, stubConditionStringCheckingDtoSchema.Properties.Count);
        Assert.IsFalse(stubConditionStringCheckingDtoSchema.Properties.Any(p => p.Value.Type == "null"));

        // Assert StubConditionStringCheckingDto
        var stubExtraDurationDtoSchema = openapi.Components.Schemas["StubExtraDurationDto"];
        Assert.AreEqual(2, stubExtraDurationDtoSchema.Properties.Count);
        Assert.IsFalse(stubConditionStringCheckingDtoSchema.Properties.Any(p => p.Value.Type == "null"));

        // Assert StubUrlConditionDto
        var stubUrlConditionDtoSchema = openapi.Components.Schemas["StubUrlConditionDto"];
        var pathSchema = stubUrlConditionDtoSchema.Properties["path"];
        Assert.AreEqual("string", pathSchema.OneOf[0].Type);
        Assert.AreEqual("StubConditionStringCheckingDto", pathSchema.OneOf[1].Title);

        var fullPathSchema = stubUrlConditionDtoSchema.Properties["fullPath"];
        Assert.AreEqual("string", fullPathSchema.OneOf[0].Type);
        Assert.AreEqual("StubConditionStringCheckingDto", fullPathSchema.OneOf[1].Title);

        var querySchema = stubUrlConditionDtoSchema.Properties["query"];
        Assert.AreEqual("string", querySchema.AdditionalProperties.OneOf[0].Type);
        Assert.AreEqual("StubConditionStringCheckingDto", querySchema.AdditionalProperties.OneOf[1].Title);

        // Assert StubConditionsDto
        var stubConditionsDtoSchema = openapi.Components.Schemas["StubConditionsDto"];
        var jsonPathItemsSchema = stubConditionsDtoSchema.Properties["jsonPath"].Items;
        Assert.AreEqual("string", jsonPathItemsSchema.OneOf[0].Type);
        Assert.AreEqual("StubJsonPathDto", jsonPathItemsSchema.OneOf[1].Title);

        var bodySchema = stubConditionsDtoSchema.Properties["body"].Items;
        Assert.AreEqual("string", bodySchema.OneOf[0].Type);
        Assert.AreEqual("StubConditionStringCheckingDto", bodySchema.OneOf[1].Title);

        var headersSchema = stubConditionsDtoSchema.Properties["headers"];
        Assert.AreEqual("string", headersSchema.AdditionalProperties.OneOf[0].Type);
        Assert.AreEqual("StubConditionStringCheckingDto", headersSchema.AdditionalProperties.OneOf[1].Title);

        var formSchema = openapi.Components.Schemas["StubFormDto"];
        Assert.AreEqual("string", formSchema.Properties["value"].OneOf[0].Type);
        Assert.AreEqual("StubConditionStringCheckingDto", formSchema.Properties["value"].OneOf[1].Title);

        var hostSchema = stubConditionsDtoSchema.Properties["host"];
        Assert.AreEqual("string", hostSchema.OneOf[0].Type);
        Assert.AreEqual("StubConditionStringCheckingDto", hostSchema.OneOf[1].Title);

        var methodSchema = stubConditionsDtoSchema.Properties["method"];
        Assert.AreEqual("string", methodSchema.OneOf[0].Type);
        Assert.AreEqual("String[]", methodSchema.OneOf[1].Title);
    }

    [TestMethod]
    public async Task GenericIntegration_Ui_ReturnsModifiedHtml()
    {
        // The URL ph-ui is not executed as stub, so it doesn't return an HTTP 500 when called.

        // Arrange
        var url = $"{TestServer.BaseAddress}ph-ui";

        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // Act
        using var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains(
            """<base href="http://localhost"><script type="text/javascript">window.rootUrl = "http://localhost";</script>"""));
    }
}
