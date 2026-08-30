using HttPlaceholder.Domain.Enums;
using HttPlaceholder.Web.Shared.Dto.v1.Export;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiExportCurlTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Export_ExportCurl_HappyFlow()
    {
        // Arrange
        var correlation = Guid.NewGuid().ToString();
        StubSource.GetCollection(null).RequestResultModels.Add(new RequestResultModel
        {
            CorrelationId = correlation,
            RequestParameters = new RequestParametersModel
            {
                Method = "DELETE",
                Url = "http://localhost:5000/some-url"
            }
        });
        var url = $"{BaseAddress}ph-api/export/requests/{correlation}?type={RequestExportType.Curl}";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RequestExportResultDto>(content);
        Assert.AreEqual(RequestExportType.Curl, result.RequestExportType);
        Assert.AreEqual("curl -X DELETE 'http://localhost:5000/some-url'", result.Result);
    }
}
