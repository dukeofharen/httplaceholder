using System.Net;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Tests.Integration.RestApi;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubStringCheckingKeywordTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [DataTestMethod]
    [DataRow("equals", "/users", "users", true)]
    [DataRow("equals", "/users", "Users", false)]
    [DataRow("equals", "/users", "userss", false)]
    [DataRow("equalsci", "/users", "users", true)]
    [DataRow("equalsci", "/users", "Users", true)]
    [DataRow("equalsci", "/users", "userss", false)]
    [DataRow("notequals", "/users", "users", false)]
    [DataRow("notequals", "/users", "Users", true)]
    [DataRow("notequals", "/users", "userss", true)]
    [DataRow("notequalsci", "/users", "users", false)]
    [DataRow("notequalsci", "/users", "Users", false)]
    [DataRow("notequalsci", "/users", "userss", true)]
    [DataRow("contains", "/users", "users", true)]
    [DataRow("contains", "ers", "users", true)]
    [DataRow("contains", "Users", "users", false)]
    [DataRow("contains", "bla", "users", false)]
    [DataRow("containsci", "/users", "users", true)]
    [DataRow("containsci", "ers", "users", true)]
    [DataRow("containsci", "Users", "users", true)]
    [DataRow("containsci", "bla", "users", false)]
    [DataRow("notcontains", "/users", "users", false)]
    [DataRow("notcontains", "ers", "users", false)]
    [DataRow("notcontains", "Users", "users", true)]
    [DataRow("notcontains", "bla", "users", true)]
    [DataRow("notcontainsci", "/users", "users", false)]
    [DataRow("notcontainsci", "ers", "users", false)]
    [DataRow("notcontainsci", "Users", "users", false)]
    [DataRow("notcontainsci", "bla", "users", true)]
    [DataRow("startswith", "/users", "users", true)]
    [DataRow("startswith", "/users", "users/1", true)]
    [DataRow("startswith", "/users", "userssss", true)]
    [DataRow("startswith", "/users", "Users/1", false)]
    [DataRow("startswith", "/users", "user", false)]
    [DataRow("startswithci", "/users", "users", true)]
    [DataRow("startswithci", "/users", "users/1", true)]
    [DataRow("startswithci", "/users", "userssss", true)]
    [DataRow("startswithci", "/users", "Users/1", true)]
    [DataRow("startswithci", "/users", "user", false)]
    [DataRow("doesnotstartwith", "/users", "users", false)]
    [DataRow("doesnotstartwith", "/users", "users/1", false)]
    [DataRow("doesnotstartwith", "/users", "userssss", false)]
    [DataRow("doesnotstartwith", "/users", "Users/1", true)]
    [DataRow("doesnotstartwith", "/users", "user", true)]
    [DataRow("doesnotstartwithci", "/users", "users", false)]
    [DataRow("doesnotstartwithci", "/users", "users/1", false)]
    [DataRow("doesnotstartwithci", "/users", "userssss", false)]
    [DataRow("doesnotstartwithci", "/users", "Users/1", false)]
    [DataRow("doesnotstartwithci", "/users", "user", true)]
    [DataRow("endswith", "users", "users", true)]
    [DataRow("endswith", "ers", "users", true)]
    [DataRow("endswith", "users", "Users", false)]
    [DataRow("endswith", "users", "user", false)]
    [DataRow("endswithci", "users", "users", true)]
    [DataRow("endswithci", "ers", "users", true)]
    [DataRow("endswithci", "users", "Users", true)]
    [DataRow("endswithci", "users", "user", false)]
    [DataRow("doesnotendwith", "users", "users", false)]
    [DataRow("doesnotendwith", "ers", "users", false)]
    [DataRow("doesnotendwith", "users", "Users", true)]
    [DataRow("doesnotendwith", "users", "user", true)]
    [DataRow("regex", "/users", "users", true)]
    [DataRow("regex", "\\/(users|customers)", "users", true)]
    [DataRow("regex", "\\/(users|customers)", "customers", true)]
    [DataRow("regex", "\\/(users|customers)", "students", false)]
    [DataRow("regexnomatches", "/users", "users", false)]
    [DataRow("regexnomatches", "\\/(users|customers)", "users", false)]
    [DataRow("regexnomatches", "\\/(users|customers)", "customers", false)]
    [DataRow("regexnomatches", "\\/(users|customers)", "students", true)]
    public async Task StubStringCheckingKeywordIntegration(
        string keyword,
        string keywordValue,
        string requestPath,
        bool shouldSucceed)
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs";
        var body = $"""
                    id: situation-01
                    conditions:
                      url:
                        path:
                          {keyword}: {keywordValue}
                    response:
                      text: OK

                    """;
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, "application/x-yaml")
        };

        // Act: add stub
        using var response = await Client.SendAsync(request);

        // Assert: check stub added
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("situation-01"));
        Assert.IsTrue(response.IsSuccessStatusCode);

        // Act / Assert: perform stub request
        using var stubResponse = await Client.GetAsync($"{BaseAddress}{requestPath}");
        if (shouldSucceed)
        {
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual("OK", await stubResponse.Content.ReadAsStringAsync());
        }
        else
        {
            Assert.AreEqual(HttpStatusCode.NotImplemented, stubResponse.StatusCode);
        }
    }
}
