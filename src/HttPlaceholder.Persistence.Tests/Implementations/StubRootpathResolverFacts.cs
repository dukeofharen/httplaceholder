using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Persistence.Implementations;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Tests.Implementations;

[TestClass]
public class StubRootPathResolverFacts
{
    private readonly Mock<IAssemblyService> _assemblyServiceMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly IOptionsMonitor<SettingsModel> _options = MockSettingsFactory.GetOptionsMonitor();
    private StubRootPathResolver _resolver;

    [TestInitialize]
    public void Initialize() =>
        _resolver = new StubRootPathResolver(
            _assemblyServiceMock.Object,
            _fileServiceMock.Object,
            _options);

    [TestCleanup]
    public void Cleanup()
    {
        _assemblyServiceMock.VerifyAll();
        _fileServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task
        StubRootPathResolverAsync_GetStubRootPaths_InputFileSet_InputFileIsDirectory_ShouldReturnInputFileAsIs()
    {
        // Arrange
        const string inputFile = @"C:\stubs";
        _options.CurrentValue.Storage.InputFile = inputFile;

        _fileServiceMock
            .Setup(m => m.IsDirectoryAsync(inputFile, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = (await _resolver.GetStubRootPathsAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(inputFile, result[0]);
    }

    [TestMethod]
    public async Task
        StubRootPathResolverAsync_GetStubRootPaths_InputFileSet_InputFileIsFile_ShouldReturnInputFileFolder()
    {
        // Arrange
        var inputFilePath =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\stubs" : "/opt/httplaceholder";

        var inputFile = Path.Combine($@"{inputFilePath}", "stubs.yml");
        _options.CurrentValue.Storage.InputFile = inputFile;

        _fileServiceMock
            .Setup(m => m.IsDirectoryAsync(inputFile, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = (await _resolver.GetStubRootPathsAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(inputFilePath, result[0]);
    }

    [TestMethod]
    public async Task
        StubRootPathResolverAsync_GetStubRootPaths_InputFileSet_FileStorageLocationIsSet_ShouldAlsoReturnFileStorageLocation()
    {
        // Arrange
        var inputFilePath =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\stubs" : "/opt/httplaceholder";

        var inputFile = Path.Combine($@"{inputFilePath}", "stubs.yml");
        _options.CurrentValue.Storage.InputFile = inputFile;

        var fileStorageLocation = "/home/httpl/.httplaceholder";
        _options.CurrentValue.Storage.FileStorageLocation = fileStorageLocation;

        _fileServiceMock
            .Setup(m => m.IsDirectoryAsync(inputFile, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = (await _resolver.GetStubRootPathsAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(inputFilePath, result[0]);
        Assert.AreEqual(fileStorageLocation, result[1]);
    }

    [DataTestMethod]
    [DataRow(",")]
    [DataRow("%%")]
    public async Task StubRootPathResolverAsync_GetStubRootPaths_InputFileSet_MultiplePaths_ShouldReturnMultiplePaths(
        string separator)
    {
        // Arrange
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var path1 = isWindows ? @"C:\stubs1" : "/opt/httplaceholder/stubs1";
        var path2 = isWindows ? @"C:\stubs2\stub.yml" : "/opt/httplaceholder/stubs2/stub.yml";
        var inputFilePath = $"{path1}{separator}{path2}";

        _options.CurrentValue.Storage.InputFile = inputFilePath;

        _fileServiceMock
            .Setup(m => m.IsDirectoryAsync(path1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _fileServiceMock
            .Setup(m => m.IsDirectoryAsync(path2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = (await _resolver.GetStubRootPathsAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(path1, result[0]);
        Assert.AreEqual(Path.GetDirectoryName(path2), result[1]);
    }

    [TestMethod]
    public async Task StubRootPathResolverAsync_GetStubRootPath_InputFileNotSet_ShouldReturnAssemblyPath()
    {
        // Arrange
        var assemblyPath = Path.Combine(@"C:\stubs\bin");

        _assemblyServiceMock
            .Setup(m => m.GetEntryAssemblyRootPath())
            .Returns(assemblyPath);

        // Act
        var result = (await _resolver.GetStubRootPathsAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(assemblyPath, result[0]);
    }
}
