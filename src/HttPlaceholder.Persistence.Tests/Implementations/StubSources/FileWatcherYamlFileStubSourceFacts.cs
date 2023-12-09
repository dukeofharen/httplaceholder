using System.IO;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class FileWatcherYamlFileStubSourceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockLogger<FileWatcherYamlFileStubSource> _mockLogger = new();
    private readonly IOptionsMonitor<SettingsModel> _options = MockSettingsFactory.GetOptionsMonitor();

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use(_options);
        _mocker.Use<ILogger<FileWatcherYamlFileStubSource>>(_mockLogger);
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task GetStubsAsync_HappyFlow()
    {
        // Arrange
        var stub1 = CreateStub();
        var stub2 = CreateStub();
        var stub3 = CreateStub();

        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file1.yml", new[]{stub1}));
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file2.yml", new[]{stub2, stub3}));

        // Act
        var result = (await source.GetStubsAsync()).ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(stub1.Id, result[0].Id);
        Assert.AreEqual(stub2.Id, result[1].Id);
        Assert.AreEqual(stub3.Id, result[2].Id);
    }

    [TestMethod]
    public async Task GetStubsOverviewAsync_HappyFlow()
    {
        // Arrange
        var stub1 = CreateStub();
        var stub2 = CreateStub();
        var stub3 = CreateStub();

        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file1.yml", new[]{stub1}));
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file2.yml", new[]{stub2, stub3}));

        // Act
        var result = (await source.GetStubsOverviewAsync()).ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(stub1.Id, result[0].Id);
        Assert.AreEqual(stub2.Id, result[1].Id);
        Assert.AreEqual(stub3.Id, result[2].Id);
    }

    [TestMethod]
    public async Task GetStubAsync_HappyFlow()
    {
        // Arrange
        var stub1 = CreateStub();
        var stub2 = CreateStub();
        var stub3 = CreateStub();

        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file1.yml", new[]{stub1}));
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file2.yml", new[]{stub2, stub3}));

        // Act
        var result = await source.GetStubAsync(stub2.Id);

        // Assert
        Assert.AreEqual(stub2.Id, result.Id);
    }

    [TestMethod]
    public void OnInputLocationUpdated_Changed_HappyFlow()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new FileSystemEventArgs(WatcherChangeTypes.Changed, "/full/path.yml", null);

        const string yaml = "- id: test123";
        fileServiceMock
            .Setup(m => m.ReadAllText(e.FullPath))
            .Returns(yaml);

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.AreEqual(1, source.Stubs.Count);

        var stubs = source.Stubs[e.FullPath].ToArray();
        Assert.AreEqual(1, stubs.Length);
        Assert.AreEqual("test123", stubs[0].Id);
    }

    [TestMethod]
    public void OnInputLocationUpdated_Created_HappyFlow_PathIsDirectory()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new FileSystemEventArgs(WatcherChangeTypes.Created, "/full/path", null);

        fileServiceMock
            .Setup(m => m.IsDirectory(e.FullPath))
            .Returns(true);

        // Act / Assert
        source.OnInputLocationUpdated(new object(), e);
    }

    [TestMethod]
    public void OnInputLocationUpdated_Created_HappyFlow_PathIsFile()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new FileSystemEventArgs(WatcherChangeTypes.Created, "/full/path.yml", null);

        fileServiceMock
            .Setup(m => m.IsDirectory(e.FullPath))
            .Returns(false);

        const string yaml = "- id: test123";
        fileServiceMock
            .Setup(m => m.ReadAllText(e.FullPath))
            .Returns(yaml);

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.AreEqual(1, source.Stubs.Count);

        var stubs = source.Stubs[e.FullPath].ToArray();
        Assert.AreEqual(1, stubs.Length);
        Assert.AreEqual("test123", stubs[0].Id);
    }

    [TestMethod]
    public void OnInputLocationUpdated_Deleted_HappyFlow_PathIsDirectory()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new FileSystemEventArgs(WatcherChangeTypes.Deleted, "/full/path", null);

        fileServiceMock
            .Setup(m => m.IsDirectory(e.FullPath))
            .Returns(true);

        var watcher = new FileSystemWatcher();
        Assert.IsTrue(source.FileSystemWatchers.TryAdd(e.FullPath, watcher));

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.AreEqual(0, source.FileSystemWatchers.Count);
    }

    [TestMethod]
    public void OnInputLocationUpdated_Deleted_HappyFlow_PathIsFile()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new FileSystemEventArgs(WatcherChangeTypes.Deleted, "/full/path.yml", null);

        fileServiceMock
            .Setup(m => m.IsDirectory(e.FullPath))
            .Returns(false);

        var watcher = new FileSystemWatcher();
        Assert.IsTrue(source.FileSystemWatchers.TryAdd(e.FullPath, watcher));

        var stub = CreateStub();
        Assert.IsTrue(source.Stubs.TryAdd(e.FullPath, new[]{stub}));

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.AreEqual(0, source.FileSystemWatchers.Count);
        Assert.AreEqual(0, source.Stubs.Count);
    }

    private static StubModel CreateStub() => new() {Id = Guid.NewGuid().ToString()};
}
