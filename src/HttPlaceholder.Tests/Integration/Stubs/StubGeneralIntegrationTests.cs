using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubGeneralIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_ReturnsXHttPlaceholderCorrelationHeader()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}bla";

        // act / assert
        using var response = await Client.GetAsync(url);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
        var header = response.Headers.First(h => h.Key == "X-HttPlaceholder-Correlation").Value.ToArray();
        Assert.AreEqual(1, header.Length);
        Assert.IsFalse(string.IsNullOrWhiteSpace(header.First()));
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_StubNotFound_ShouldReturn501WithContent()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}locatieserver/v3/suggest?q=9752EX";

        // act / assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content));
        Assert.IsTrue(content.Contains("<html"));
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}