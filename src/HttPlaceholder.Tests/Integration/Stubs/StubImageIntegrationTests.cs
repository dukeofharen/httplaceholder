using System.IO;
using System.Linq;
using SixLabors.ImageSharp;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubImageIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize()
    {
        InitializeStubIntegrationTest("Resources/integration.yml");
        FileServiceMock
            .Setup(m => m.GetTempPath())
            .Returns(OperatingSystem.IsWindows() ? @"C:\Windows\Temp" : "/tmp");
    }

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_StubImage_Jpeg()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}stub-image.jpg";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual("image/jpeg", response.Content.Headers.Single(h => h.Key == "Content-Type").Value.Single());
        await using var ms = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
        using var image = await Image.LoadAsync(ms);
        Assert.AreEqual(1024, image.Width);
        Assert.AreEqual(256, image.Height);
    }

    [TestMethod]
    public async Task StubIntegration_StubImage_Png()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}stub-image.png";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual("image/png", response.Content.Headers.Single(h => h.Key == "Content-Type").Value.Single());
        await using var ms = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
        using var image = await Image.LoadAsync(ms);
        Assert.AreEqual(1024, image.Width);
        Assert.AreEqual(256, image.Height);
    }

    [TestMethod]
    public async Task StubIntegration_StubImage_Bmp()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}stub-image.bmp";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual("image/bmp", response.Content.Headers.Single(h => h.Key == "Content-Type").Value.Single());
        await using var ms = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
        using var image = await Image.LoadAsync(ms);
        Assert.AreEqual(1024, image.Width);
        Assert.AreEqual(256, image.Height);
    }

    [TestMethod]
    public async Task StubIntegration_StubImage_Gif()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}stub-image.gif";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual("image/gif", response.Content.Headers.Single(h => h.Key == "Content-Type").Value.Single());
        await using var ms = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
        using var image = await Image.LoadAsync(ms);
        Assert.AreEqual(1024, image.Width);
        Assert.AreEqual(256, image.Height);
    }
}
