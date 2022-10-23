using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Dto.v1.Requests;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiStubGenerationTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_StubGeneration_HappyFlow()
    {
        // Arrange
        ClientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns("127.0.0.1");

        ClientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns("localhost");

        // Do a call to a non-existent stub
        var response = await Client.SendAsync(CreateTestStubRequest());
        var correlationId = response.Headers.Single(h => h.Key == HeaderKeys.XHttPlaceholderCorrelation).Value.Single();

        // Register a new stub for the failed request
        var url = $"{BaseAddress}ph-api/requests/{correlationId}/stubs";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent("{}", Encoding.UTF8, MimeTypes.JsonMime)
        };
        response = await Client.SendAsync(apiRequest);
        response.EnsureSuccessStatusCode();

        // Do the original stub request again; it should succeed now
        response = await Client.SendAsync(CreateTestStubRequest());
        response.EnsureSuccessStatusCode();

        // Check the actual added stub
        var addedStub = StubSource.StubModels.Single();
        Assert.AreEqual("/test123", ((StubConditionStringCheckingModel)addedStub.Conditions.Url.Path).StringEquals);

        Assert.AreEqual("val1",
            ((StubConditionStringCheckingModel)addedStub.Conditions.Url.Query["query1"]).StringEquals);
        Assert.AreEqual("val2",
            ((StubConditionStringCheckingModel)addedStub.Conditions.Url.Query["query2"]).StringEquals);

        Assert.AreEqual("POST", addedStub.Conditions.Method);

        var formDict = addedStub.Conditions.Form.ToDictionary(f => f.Key, f => f.Value);
        Assert.AreEqual("val1", ((StubConditionStringCheckingModel)formDict["form1"]).StringEquals);
        Assert.AreEqual("val2", ((StubConditionStringCheckingModel)formDict["form2"]).StringEquals);

        Assert.AreEqual("abc123",
            ((StubConditionStringCheckingModel)addedStub.Conditions.Headers["X-Api-Key"]).StringEquals);

        Assert.AreEqual("127.0.0.1", addedStub.Conditions.ClientIp);

        Assert.IsFalse(addedStub.Conditions.Url.IsHttps.HasValue && addedStub.Conditions.Url.IsHttps.Value);

        Assert.AreEqual("localhost", ((StubConditionStringCheckingModel)addedStub.Conditions.Host).StringEquals);

        Assert.AreEqual("duco", addedStub.Conditions.BasicAuthentication.Username);
        Assert.AreEqual("pass", addedStub.Conditions.BasicAuthentication.Password);
    }

    [TestMethod]
    public async Task RestApiIntegration_StubGeneration_HappyFlow_OnlyReturnStub()
    {
        // Arrange
        ClientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns("127.0.0.1");

        ClientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns("localhost");

        // Do a call to a non-existent stub
        var response = await Client.SendAsync(CreateTestStubRequest());
        var correlationId = response.Headers.Single(h => h.Key == HeaderKeys.XHttPlaceholderCorrelation).Value.Single();

        // Register a new stub for the failed request
        var url = $"{BaseAddress}ph-api/requests/{correlationId}/stubs";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(
                JsonConvert.SerializeObject(new CreateStubForRequestInputDto {DoNotCreateStub = true}),
                Encoding.UTF8, MimeTypes.JsonMime)
        };
        response = await Client.SendAsync(apiRequest);
        response.EnsureSuccessStatusCode();

        // Check that the stub is not added to the stub source.
        Assert.AreEqual(0, StubSource.StubModels.Count);
    }

    private HttpRequestMessage CreateTestStubRequest()
    {
        var clientRequest = new HttpRequestMessage
        {
            RequestUri = new Uri($"{BaseAddress}test123?query1=val1&query2=val2"),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"form1", "val1"}, {"form2", "val2"}
            })
        };
        clientRequest.Headers.Add("X-Api-Key", "abc123");

        var auth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("duco:pass"));
        clientRequest.Headers.Add("Authorization", auth);
        return clientRequest;
    }
}
