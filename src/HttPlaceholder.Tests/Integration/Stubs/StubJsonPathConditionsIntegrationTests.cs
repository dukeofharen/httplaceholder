using System.Net;
using System.Net.Http;
using System.Text;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubJsonPathConditionsIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularPut_JsonPathText_ValidateJsonPath_HappyFlow()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}users";
        const string body = """
                            {
                              "firstName": "John",
                              "lastName" : "doe",
                              "age"      : 26,
                              "address"  : {
                                "streetAddress": "naist street",
                                "city"         : "Nara",
                                "postalCode"   : "630-0192"
                              },
                              "phoneNumbers": [
                                {
                                  "type"  : "iPhone",
                                  "number": "0123-4567-8888"
                                },
                                {
                                  "type"  : "home",
                                  "number": "0123-4567-8910"
                                }
                              ]
                            }
                            """;
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime),
            Method = HttpMethod.Put,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("jpath-string-test-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_RegularPut_JsonPathText_ValidateJsonPath_StubNotFound()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}users";
        const string body = """
                            {
                              "firstName": "John",
                              "lastName" : "doe",
                              "age"      : 26,
                              "address"  : {
                                "streetAddress": "naist street",
                                "city"         : "Nara",
                                "postalCode"   : "630-0192"
                              },
                              "phoneNumbers": [
                                {
                                  "type"  : "Android",
                                  "number": "0123-4567-8888"
                                },
                                {
                                  "type"  : "home",
                                  "number": "0123-4567-8910"
                                }
                              ]
                            }
                            """;
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime),
            Method = HttpMethod.Put,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_RegularPut_JsonPathObject_ValidateJsonPath_HappyFlow()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}users";
        const string body = """
                            {"people": [{
                              "firstName": "John",
                              "age": 29,
                              "achievements": [
                                {
                                  "name": "Just an average guy"
                                }
                              ]
                            }]}
                            """;
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime),
            Method = HttpMethod.Put,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("jpath-object-test-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_RegularPut_JsonPathObject_ValidateJsonPath_StubNotFound()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}users";
        const string body = """
                            {"people": [{
                              "firstName": "Marc",
                              "age": 29,
                              "achievements": [
                                {
                                  "name": "Just an average guy"
                                }
                              ]
                            }]}
                            """;
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime),
            Method = HttpMethod.Put,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
