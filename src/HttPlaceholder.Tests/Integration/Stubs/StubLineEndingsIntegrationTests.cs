using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubLineEndingsIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_LineEndings_Unix()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}/unix-line-endings";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("text\nwith\nunix\nline\nendings\n", content);
    }

    [TestMethod]
    public async Task StubIntegration_LineEndings_Windows()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}/windows-line-endings";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("text\r\nwith\r\nwindows\r\nline\r\nendings\r\n", content);
    }
}