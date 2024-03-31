using System.Net;
using System.Net.Http;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubJsonConditionsIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_Json_Object_Valid()
    {
        // Arrange
        const string jsonToPost = """
                                  {
                                    "username": "username",
                                    "subObject": {
                                      "strValue": "stringInput",
                                      "boolValue": true,
                                      "doubleValue": 1.23,
                                      "dateTimeValue": "2021-04-16T21:23:03",
                                      "intValue": 3,
                                      "nullValue": null,
                                      "arrayValue": [
                                        "val1",
                                        {
                                          "subKey1": "subValue1",
                                          "subKey2": "subValue2"
                                        }
                                      ]
                                    }
                                  }

                                  """;

        var url = $"{TestServer.BaseAddress}json";
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(jsonToPost) };

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK JSON OBJECT!", content);
    }

    [TestMethod]
    public async Task StubIntegration_Json_Object_Invalid()
    {
        // Arrange
        const string jsonToPost = """
                                  {
                                    "username": "username",
                                    "subObject": {
                                      "strValue": "stringInput",
                                      "boolValue": true,
                                      "doubleValue": 1.25,
                                      "dateTimeValue": "2021-04-16T21:23:03",
                                      "intValue": 3,
                                      "nullValue": null,
                                      "arrayValue": [
                                        "val1",
                                        {
                                          "subKey1": "subValue1",
                                          "subKey2": "subValue2"
                                        }
                                      ]
                                    }
                                  }

                                  """;

        var url = $"{TestServer.BaseAddress}json";
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(jsonToPost) };

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_Json_Array_Valid()
    {
        // Arrange
        const string jsonToPost = """
                                  [
                                      "val1",
                                      3,
                                      1.46,
                                      "2021-04-17T13:16:54",
                                      {
                                          "stringVal": "val1",
                                          "intVal": 55
                                      }
                                  ]

                                  """;

        var url = $"{TestServer.BaseAddress}json";
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(jsonToPost) };

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK JSON ARRAY!", content);
    }

    [TestMethod]
    public async Task StubIntegration_Json_Array_Invalid()
    {
        // Arrange
        const string jsonToPost = """
                                  [
                                      "val1",
                                      3,
                                      1.48,
                                      "2021-04-17T13:16:54",
                                      {
                                          "stringVal": "val1",
                                          "intVal": 55
                                      }
                                  ]

                                  """;

        var url = $"{TestServer.BaseAddress}json";
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(jsonToPost) };

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
