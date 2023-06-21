using System.Linq;
using System.Net;
using System.Text;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubFileIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_File_FileBinary_HappyFlow()
    {
        // arrange
        const string fileContents = "File contents yo! ((uuid))";
        var url = $"{TestServer.BaseAddress}text-binary.bin";
        Options.CurrentValue.Stub.AllowGlobalFileSearch = true;

        FileServiceMock
            .Setup(m => m.FileExistsAsync("text.bin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        FileServiceMock
            .Setup(m => m.ReadAllBytesAsync("text.bin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(fileContents));

        // act / assert
        using var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(fileContents, content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/octet-stream", response.Content.Headers.Single(h => h.Key == "Content-Type").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_File_FileTextWithVariable_HappyFlow()
    {
        // arrange
        const string fileContents = "File contents yo! ((query:key))";
        var url = $"{TestServer.BaseAddress}text.txt?key=value123";
        Options.CurrentValue.Stub.AllowGlobalFileSearch = true;

        FileServiceMock
            .Setup(m => m.FileExistsAsync("text.txt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        FileServiceMock
            .Setup(m => m.ReadAllBytesAsync("text.txt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(fileContents));

        // act / assert
        using var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("File contents yo! value123", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("text/plain", response.Content.Headers.Single(h => h.Key == "Content-Type").Value.Single());
    }
}
