using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Common;
using HttPlaceholder.Common.FileWatchers;
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
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file1.yml", new[] { stub1 }));
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file2.yml", new[] { stub2, stub3 }));

        // Act
        var result = (await source.GetStubsAsync()).ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(stub1.Id, result[0].Stub.Id);
        Assert.AreEqual("/path/to/file1.yml", result[0].Metadata[StubMetadataKeys.Filename]);
        Assert.AreEqual(stub2.Id, result[1].Stub.Id);
        Assert.AreEqual("/path/to/file2.yml", result[1].Metadata[StubMetadataKeys.Filename]);
        Assert.AreEqual(stub3.Id, result[2].Stub.Id);
        Assert.AreEqual("/path/to/file2.yml", result[2].Metadata[StubMetadataKeys.Filename]);
    }

    [TestMethod]
    public async Task GetStubsOverviewAsync_HappyFlow()
    {
        // Arrange
        var stub1 = CreateStub();
        var stub2 = CreateStub();
        var stub3 = CreateStub();

        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file1.yml", new[] { stub1 }));
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file2.yml", new[] { stub2, stub3 }));

        // Act
        var result = (await source.GetStubsOverviewAsync()).ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(stub1.Id, result[0].Stub.Id);
        Assert.AreEqual("/path/to/file1.yml", result[0].Metadata[StubMetadataKeys.Filename]);
        Assert.AreEqual(stub2.Id, result[1].Stub.Id);
        Assert.AreEqual("/path/to/file2.yml", result[1].Metadata[StubMetadataKeys.Filename]);
        Assert.AreEqual(stub3.Id, result[2].Stub.Id);
        Assert.AreEqual("/path/to/file2.yml", result[2].Metadata[StubMetadataKeys.Filename]);
    }

    [TestMethod]
    public async Task GetStubAsync_HappyFlow()
    {
        // Arrange
        var stub1 = CreateStub();
        var stub2 = CreateStub();
        var stub3 = CreateStub();

        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file1.yml", new[] { stub1 }));
        Assert.IsTrue(source.Stubs.TryAdd("/path/to/file2.yml", new[] { stub2, stub3 }));

        // Act
        var result = await source.GetStubAsync(stub2.Id);

        // Assert
        Assert.IsTrue(result.HasValue);
        Assert.AreEqual(stub2.Id, result.Value.Stub.Id);
        Assert.AreEqual("/path/to/file2.yml", result.Value.Metadata[StubMetadataKeys.Filename]);
    }

    [TestMethod]
    public void SetupWatcherForLocation_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        const string location = "/path/to/stubs";
        var (builderMock, watcher) = ConfigureWatcherFactory();

        // Act
        source.SetupWatcherForLocation(location);

        // Assert
        Assert.AreEqual(1, source.FileSystemWatchers.Count);
        Assert.AreEqual(watcher, source.FileSystemWatchers[location]);
        builderMock.Verify(m =>
            m.SetPathOrFilters(location,
                It.Is<IEnumerable<string>>(e => e.Any(ext => ext == ".yml" || ext == ".yaml"))));
        builderMock.Verify(m => m.SetNotifyFilters(NotifyFilters.CreationTime
                                                   | NotifyFilters.DirectoryName
                                                   | NotifyFilters.FileName
                                                   | NotifyFilters.LastWrite
                                                   | NotifyFilters.Size
                                                   | NotifyFilters.Attributes
                                                   | NotifyFilters.Security));
        builderMock.Verify(m => m.SetOnChanged(It.IsAny<Action<object, FileSystemEventArgs>>()));
        builderMock.Verify(m => m.SetOnCreated(It.IsAny<Action<object, FileSystemEventArgs>>()));
        builderMock.Verify(m => m.SetOnDeleted(It.IsAny<Action<object, FileSystemEventArgs>>()));
        builderMock.Verify(m => m.SetOnRenamed(It.IsAny<Action<object, RenamedEventArgs>>()));
        builderMock.Verify(m => m.SetOnError(It.IsAny<Action<object, ErrorEventArgs>>()));
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

        _mocker.GetMock<IStubNotify>().Verify(m => m.ReloadStubsAsync(null, It.IsAny<CancellationToken>()));
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

        _mocker.GetMock<IStubNotify>().Verify(m => m.ReloadStubsAsync(null, It.IsAny<CancellationToken>()));
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
        Assert.IsTrue(source.Stubs.TryAdd(e.FullPath, new[] { stub }));

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.AreEqual(0, source.FileSystemWatchers.Count);
        Assert.AreEqual(0, source.Stubs.Count);

        _mocker.GetMock<IStubNotify>().Verify(m => m.ReloadStubsAsync(null, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public void OnInputLocationUpdated_Renamed_InputIsDirectory_ShouldDoNothing()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new RenamedEventArgs(WatcherChangeTypes.Renamed, "/full/path", "new.yml", "old.yml");

        fileServiceMock
            .Setup(m => m.IsDirectory(e.FullPath))
            .Returns(true);

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.IsFalse(_mockLogger.HasEntries());
    }

    [TestMethod]
    public void OnInputLocationUpdated_Renamed_InputIsFile_ExtensionNotSupported()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new RenamedEventArgs(WatcherChangeTypes.Renamed, "/full/path", "new.txt", "old.yml");

        fileServiceMock
            .Setup(m => m.IsDirectory(e.FullPath))
            .Returns(false);

        Assert.IsTrue(source.Stubs.TryAdd(e.OldFullPath, new[] { CreateStub() }));

        var watcher = new FileSystemWatcher();
        Assert.IsTrue(source.FileSystemWatchers.TryAdd(e.OldFullPath, watcher));

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, $"File {e.OldFullPath} renamed to {e.FullPath}."));
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, $"Removed stub '{e.OldFullPath}'."));
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug,
            $"File {e.FullPath} not supported. Supported file extensions: .yml, .yaml"));
        Assert.IsTrue(source.Stubs.IsEmpty);
        Assert.IsTrue(source.FileSystemWatchers.IsEmpty);
    }

    [TestMethod]
    public void OnInputLocationUpdated_Renamed_InputIsFile_ExtensionSupported()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        var e = new RenamedEventArgs(WatcherChangeTypes.Renamed, "/full/path", "new.yml", "old.yml");

        fileServiceMock
            .Setup(m => m.IsDirectory(e.FullPath))
            .Returns(false);

        Assert.IsTrue(source.Stubs.TryAdd(e.OldFullPath, new[] { CreateStub() }));

        Assert.IsTrue(source.FileSystemWatchers.TryAdd(e.OldFullPath, new FileSystemWatcher()));

        const string yaml = "- id: test123";
        fileServiceMock
            .Setup(m => m.ReadAllText(e.FullPath))
            .Returns(yaml);

        var (_, watcher) = ConfigureWatcherFactory();

        // Act
        source.OnInputLocationUpdated(new object(), e);

        // Assert
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, $"File {e.OldFullPath} renamed to {e.FullPath}."));
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, $"Removed stub '{e.OldFullPath}'."));
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, $"Trying to add and parse stubs for '{e.FullPath}'."));
        Assert.AreEqual(1, source.FileSystemWatchers.Count);
        Assert.AreEqual(watcher, source.FileSystemWatchers[e.FullPath]);
        Assert.AreEqual(1, source.Stubs.Count);
        Assert.AreEqual("test123", source.Stubs[e.FullPath].Single().Id);

        _mocker.GetMock<IStubNotify>().Verify(m => m.ReloadStubsAsync(null, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public void TryRemoveWatcher_WatcherFound()
    {
        // Arrange
        const string fullPath = "/path/to/folder";
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        Assert.IsTrue(source.FileSystemWatchers.TryAdd(fullPath, new FileSystemWatcher()));

        // Act
        var result = source.TryRemoveWatcher(fullPath);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(source.FileSystemWatchers.IsEmpty);
    }

    [TestMethod]
    public void TryRemoveWatcher_WatcherNotFound()
    {
        // Arrange
        const string fullPath = "/path/to/folder";
        var source = _mocker.CreateInstance<FileWatcherYamlFileStubSource>();
        Assert.IsTrue(source.FileSystemWatchers.TryAdd(fullPath, new FileSystemWatcher()));

        // Act
        var result = source.TryRemoveWatcher(fullPath + "1");

        // Assert
        Assert.IsFalse(result);
        Assert.IsFalse(source.FileSystemWatchers.IsEmpty);
    }

    private static StubModel CreateStub() => new() { Id = Guid.NewGuid().ToString() };

    private (Mock<IFileWatcherBuilder> BuilderMock, FileSystemWatcher Watcher) ConfigureWatcherFactory()
    {
        var factoryMock = _mocker.GetMock<IFileWatcherBuilderFactory>();
        var watcher = new FileSystemWatcher();
        var builderMock = new Mock<IFileWatcherBuilder>();

        factoryMock
            .Setup(m => m.CreateBuilder())
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.SetPathOrFilters(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.SetNotifyFilters(It.IsAny<NotifyFilters>()))
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.SetOnChanged(It.IsAny<Action<object, FileSystemEventArgs>>()))
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.SetOnCreated(It.IsAny<Action<object, FileSystemEventArgs>>()))
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.SetOnDeleted(It.IsAny<Action<object, FileSystemEventArgs>>()))
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.SetOnRenamed(It.IsAny<Action<object, RenamedEventArgs>>()))
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.SetOnError(It.IsAny<Action<object, ErrorEventArgs>>()))
            .Returns(builderMock.Object);

        builderMock
            .Setup(m => m.Build())
            .Returns(watcher);

        return (builderMock, watcher);
    }
}
