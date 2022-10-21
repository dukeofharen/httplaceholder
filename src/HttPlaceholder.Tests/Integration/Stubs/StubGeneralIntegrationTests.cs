using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubGeneralIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize()
    {
        Settings.Gui.EnableUserInterface = true;
        InitializeStubIntegrationTest("Resources/integration.yml");
    }

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_ReturnsXHttPlaceholderExecutedStubHeader()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}test";
        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // Act
        using var response = await Client.SendAsync(request);

        // Assert
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.AreEqual("test-stub",
            response.Headers.Single(h => h.Key == "X-HttPlaceholder-ExecutedStub").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_ReturnsXHttPlaceholderCorrelationHeader()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}bla";

        const string page501 = "<html><body>NOT IMPLEMENTED</body></html>";
        FileServiceMock
            .Setup(m => m.ReadAllText(It.Is<string>(p => p.Contains("StubNotConfigured.html"))))
            .Returns(page501);

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

        const string page501 = "<html><body>NOT IMPLEMENTED</body></html>";
        FileServiceMock
            .Setup(m => m.ReadAllText(It.Is<string>(p => p.Contains("StubNotConfigured.html"))))
            .Returns(page501);

        // act / assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content));
        Assert.IsTrue(content.Contains("<html"));
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
