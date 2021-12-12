using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.Implementations.StubSources;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class YamlFileStubSourceFacts
{
    private readonly IOptions<SettingsModel> _options = MockSettingsFactory.GetSettings();
    private readonly Mock<ILogger<YamlFileStubSource>> _loggerMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly Mock<IStubModelValidator> _stubModelValidatorMock = new();
    private YamlFileStubSource _source;

    [TestInitialize]
    public void Initialize() =>
        _source = new YamlFileStubSource(
            _fileServiceMock.Object,
            _loggerMock.Object,
            _options,
            _stubModelValidatorMock.Object);

    [TestCleanup]
    public void Cleanup()
    {
        _loggerMock.VerifyAll();
        _fileServiceMock.VerifyAll();
        _stubModelValidatorMock.VerifyAll();
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_NoInputFileSet_ShouldReadFilesFromCurrentDirectory()
    {
        // arrange
        const string currentDirectory = @"C:\stubs";
        var files = new[] {$@"{currentDirectory}\file1.yml", $@"{currentDirectory}\file2.yml"};

        _fileServiceMock
            .Setup(m => m.GetCurrentDirectory())
            .Returns(currentDirectory);

        _fileServiceMock
            .Setup(m => m.GetFiles(currentDirectory,
                It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[0]))
            .Returns(TestResources.YamlFile1);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[1]))
            .Returns(TestResources.YamlFile2);

        // act
        var result = await _source.GetStubsAsync();

        // assert
        var ids = result.Select(s => s.Id).ToArray();
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
        Assert.AreEqual("situation-post-01", ids[2]);
    }

    [TestMethod]
    public async Task
        YamlFileStubSource_GetStubsAsync_NoInputFileSet_ShouldReadFilesFromCurrentDirectory_NoFilesFound_ShouldReturnEmptyList()
    {
        // arrange
        const string currentDirectory = @"C:\stubs";
        _fileServiceMock
            .Setup(m => m.GetCurrentDirectory())
            .Returns(currentDirectory);

        _fileServiceMock
            .Setup(m => m.GetFiles(currentDirectory, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(new string[0]);

        // act
        var result = await _source.GetStubsAsync();

        // assert
        Assert.AreEqual(0, result.Count());
    }

    [DataTestMethod]
    [DataRow(",")]
    [DataRow("%%")]
    public async Task YamlFileStubSource_GetStubsAsync_InputFileSet_ShouldReadFilesFromThatDirectory(
        string separator)
    {
        // arrange
        var files = new[] {@"C:\stubs\file1.yml", @"C:\stubs\file2.yml"};
        _options.Value.Storage.InputFile = string.Join(separator, files);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[0]))
            .Returns(TestResources.YamlFile1);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[1]))
            .Returns(TestResources.YamlFile2);

        // act
        var result = await _source.GetStubsAsync();

        // assert
        var ids = result.Select(s => s.Id).ToArray();
        Assert.AreEqual(3, ids.Length);
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
        Assert.AreEqual("situation-post-01", ids[2]);
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_OneYamlFileIsInvalid_ShouldContinueAnyway()
    {
        // arrange
        var files = new[] {@"C:\stubs\file1.yml", @"C:\stubs\file2.yml"};
        _options.Value.Storage.InputFile = string.Join(",", files);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[0]))
            .Returns(TestResources.YamlFile1);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[1]))
            .Returns("THIS IS INVALID YAML!");

        // act
        var result = await _source.GetStubsAsync();

        // assert
        var ids = result.Select(s => s.Id).ToArray();
        Assert.AreEqual(2, ids.Length);
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
    }

    [TestMethod]
    public async Task
        YamlFileStubSource_GetStubsAsync_InputFileSet_InputFileIsDirectory_ShouldReadFilesFromThatDirectory()
    {
        // arrange
        const string inputFile = @"C:\stubs";
        _options.Value.Storage.InputFile = inputFile;

        var files = new[] {@"C:\stubs\file1.yml", @"C:\stubs\file2.yml"};

        _fileServiceMock
            .Setup(m => m.GetFiles(inputFile, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        _fileServiceMock
            .Setup(m => m.IsDirectory(inputFile))
            .Returns(true);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[0]))
            .Returns(TestResources.YamlFile1);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[1]))
            .Returns(TestResources.YamlFile2);

        // act
        var result = await _source.GetStubsAsync();

        // assert
        var ids = result.Select(s => s.Id).ToArray();
        Assert.AreEqual("situation-01", ids[0]);
        Assert.AreEqual("situation-02", ids[1]);
        Assert.AreEqual("situation-post-01", ids[2]);
        _stubModelValidatorMock.Verify(m => m.ValidateStubModel(It.IsAny<StubModel>()), Times.Exactly(3));
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_StubsHaveNoId_IdShouldBeCalculated()
    {
        // arrange
        const string inputFile = @"C:\stubs";
        _options.Value.Storage.InputFile = inputFile;

        var files = new[] {@"C:\stubs\file3.yml"};

        _fileServiceMock
            .Setup(m => m.GetFiles(inputFile, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        _fileServiceMock
            .Setup(m => m.IsDirectory(inputFile))
            .Returns(true);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[0]))
            .Returns(TestResources.YamlFile3);

        // act
        var result = await _source.GetStubsAsync();

        // assert
        var ids = result.Select(s => s.Id).ToArray();
        Assert.AreEqual("997fdb292414c5c102ee78b4dacd2e2a", ids[0]);
        Assert.AreEqual("bbcd60e6a97b00ab25bb732d14027f19", ids[1]);
    }

    [TestMethod]
    public async Task YamlFileStubSource_GetStubsAsync_YamlIsNotInFormOfArray_ShouldParseStubAnyway()
    {
        // arrange
        const string inputFile = @"C:\stubs";
        _options.Value.Storage.InputFile = inputFile;

        var files = new[] {@"C:\stubs\file4.yml"};

        _fileServiceMock
            .Setup(m => m.GetFiles(inputFile, It.Is<string[]>(e => e[0] == ".yml" && e[1] == ".yaml")))
            .Returns(files);

        _fileServiceMock
            .Setup(m => m.IsDirectory(inputFile))
            .Returns(true);

        _fileServiceMock
            .Setup(m => m.ReadAllText(files[0]))
            .Returns(TestResources.YamlFile4);

        // act
        var result = await _source.GetStubsAsync();

        // assert
        var ids = result.Select(s => s.Id).ToArray();
        Assert.AreEqual("situation-01", ids[0]);
    }
}