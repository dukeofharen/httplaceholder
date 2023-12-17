using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class YamlFileStubSourceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockLogger<YamlFileStubSource> _mockLogger = new();
    private readonly IOptionsMonitor<SettingsModel> _options = MockSettingsFactory.GetOptionsMonitor();

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use(_options);
        _mocker.Use<ILogger<YamlFileStubSource>>(_mockLogger);
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_NoInputFileSet_ShouldReadFilesFromCurrentDirectory()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        const string currentDirectory = @"C:\stubs";
        var files = new[] {$@"{currentDirectory}\file1.yml", $@"{currentDirectory}\file2.yml"};

        fileServiceMock
            .Setup(m => m.GetCurrentDirectory())
            .Returns(currentDirectory);

        fileServiceMock
            .Setup(m => m.GetFiles(currentDirectory,
                It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile1);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[1], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile2);

        // Act
        var result = await source.GetStubsAsync(null, CancellationToken.None);

        // Assert
        var ids = result.Select(s => s.Stub.Id).ToArray();
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
        Assert.AreEqual("situation-post-01", ids[2]);
        AssertNoWarningsOrErrors();
    }

    [TestMethod]
    public async Task
        YamlFileStubSource_GetStubsAsync_NoInputFileSet_ShouldReadFilesFromCurrentDirectory_NoFilesFound_ShouldReturnEmptyList()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();

        const string currentDirectory = @"C:\stubs";
        fileServiceMock
            .Setup(m => m.GetCurrentDirectory())
            .Returns(currentDirectory);

        fileServiceMock
            .Setup(m => m.GetFiles(currentDirectory, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(Array.Empty<string>());

        // Act
        var result = await source.GetStubsAsync(null, CancellationToken.None);

        // Assert
        Assert.AreEqual(0, result.Count());
        AssertNoWarningsOrErrors();
    }

    [DataTestMethod]
    [DataRow(",")]
    [DataRow("%%")]
    public async Task YamlFileStubSource_GetStubsAsync_InputFileSet_ShouldReadFilesFromThatDirectory(
        string separator)
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var files = new[] {@"C:\stubs\file1.yml", @"C:\stubs\file2.yml"};
        _options.CurrentValue.Storage.InputFile = string.Join(separator, files);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile1);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[1], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile2);

        // Act
        var result = await source.GetStubsAsync(null, CancellationToken.None);

        // Assert
        var ids = result.Select(s => s.Stub.Id).ToArray();
        Assert.AreEqual(3, ids.Length);
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
        Assert.AreEqual("situation-post-01", ids[2]);
        AssertNoWarningsOrErrors();
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_OneYamlFileIsInvalid_ShouldContinueAnyway()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var files = new[] {@"C:\stubs\file1.yml", @"C:\stubs\file2.yml"};
        _options.CurrentValue.Storage.InputFile = string.Join(",", files);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile1);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[1], It.IsAny<CancellationToken>()))
            .ReturnsAsync("THIS IS INVALID YAML!");

        // Act
        var result = await source.GetStubsAsync(null, CancellationToken.None);

        // Assert
        var ids = result.Select(s => s.Stub.Id).ToArray();
        Assert.AreEqual(2, ids.Length);
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
    }

    [TestMethod]
    public async Task
        YamlFileStubSource_GetStubsAsync_InputFileSet_InputFileIsDirectory_ShouldReadFilesFromThatDirectory()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var stubModelValidatorMock = _mocker.GetMock<IStubModelValidator>();

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        const string inputFile = @"C:\stubs";
        _options.CurrentValue.Storage.InputFile = inputFile;

        var files = new[] {@"C:\stubs\file1.yml", @"C:\stubs\file2.yml"};

        fileServiceMock
            .Setup(m => m.GetFiles(inputFile, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        fileServiceMock
            .Setup(m => m.IsDirectory(inputFile))
            .Returns(true);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile1);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[1], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile2);

        // Act
        var result = await source.GetStubsAsync(null, CancellationToken.None);

        // Assert
        var ids = result.Select(s => s.Stub.Id).ToArray();
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
        Assert.AreEqual("situation-post-01", ids[2]);
        stubModelValidatorMock.Verify(m => m.ValidateStubModel(It.IsAny<StubModel>()), Times.Exactly(3));
        AssertNoWarningsOrErrors();
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_StubsHaveNoId_ShouldLogWarnings()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        const string inputFile = @"C:\stubs";
        _options.CurrentValue.Storage.InputFile = inputFile;

        var files = new[] {@"C:\stubs\file3.yml"};

        fileServiceMock
            .Setup(m => m.GetFiles(inputFile, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        fileServiceMock
            .Setup(m => m.IsDirectory(inputFile))
            .Returns(true);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile3);

        // Act
        var result = await source.GetStubsAsync(null, CancellationToken.None);

        // Assert
        var ids = result.Select(s => s.Stub.Id).ToArray();
        Assert.AreEqual(0, ids.Length);
        Assert.AreEqual(2, _mockLogger
            .Entries
            .Count(e => e.LogLevel == LogLevel.Warning &&
                        e.State == $"Stub in file '{files[0]}' has no 'id' field defined, so is not a valid stub."));
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_YamlIsNotInFormOfArray_ShouldParseStubAnyway()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        const string inputFile = @"C:\stubs";
        _options.CurrentValue.Storage.InputFile = inputFile;

        var files = new[] {@"C:\stubs\file4.yml"};

        fileServiceMock
            .Setup(m => m.GetFiles(inputFile, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        fileServiceMock
            .Setup(m => m.IsDirectory(inputFile))
            .Returns(true);

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(files[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile4);

        // Act
        var result = await source.GetStubsAsync(null, CancellationToken.None);

        // Assert
        var ids = result.Select(s => s.Stub.Id).ToArray();
        Assert.AreEqual("situation-01", ids[0]);
        AssertNoWarningsOrErrors();
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_StubsHaveValidationErrors_ShouldLogWarnings()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var stubModelValidatorMock = _mocker.GetMock<IStubModelValidator>();

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        fileServiceMock
            .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        const string inputFile = @"C:\stubs\file1.yml";
        _options.CurrentValue.Storage.InputFile = inputFile;

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(inputFile, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile1);

        stubModelValidatorMock
            .Setup(m => m.ValidateStubModel(It.IsAny<StubModel>()))
            .Returns(new[] {"validation error"});

        // Act
        var result = (await source.GetStubsAsync(null, CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        var warnings = _mockLogger.Entries
            .Where(e => e.LogLevel == LogLevel.Warning && e.State.Contains("Validation warnings"))
            .Select(e => e.State)
            .ToArray();

        Assert.AreEqual("Validation warnings encountered for stub 'situation-01':\n- validation error", warnings[0]);
        Assert.AreEqual("Validation warnings encountered for stub 'situation-02':\n- validation error", warnings[1]);
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsOverviewAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<YamlFileStubSource>();
        var fileServiceMock = _mocker.GetMock<IFileService>();

        fileServiceMock
                    .Setup(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

        const string inputFile = @"C:\stubs\file1.yml";
        _options.CurrentValue.Storage.InputFile = inputFile;

        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(inputFile, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestResources.YamlFile1);

        // Act
        var result = (await source.GetStubsOverviewAsync(null, CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        var result1 = result[0];
        Assert.AreEqual("situation-01", result1.Stub.Id);
        Assert.AreEqual("01-get", result1.Stub.Tenant);
        Assert.IsTrue(result1.Stub.Enabled);
        AssertNoWarningsOrErrors();
    }

    private void AssertNoWarningsOrErrors() =>
        Assert.IsFalse(_mockLogger.Entries.Any(e =>
            e.LogLevel is LogLevel.Warning or LogLevel.Error or LogLevel.Critical));
}
