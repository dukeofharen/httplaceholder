using System.IO;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Persistence.FileSystem;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class FileSystemStubSourceFacts
{
    private const string StorageFolder = @"C:\storage";

    private readonly AutoMocker _mocker = new();
    private readonly IOptionsMonitor<SettingsModel> _options = MockSettingsFactory.GetOptionsMonitor();

    [TestInitialize]
    public void Initialize()
    {
        _options.CurrentValue.Storage.FileStorageLocation = StorageFolder;
        _options.CurrentValue.Storage.OldRequestsQueueLength = 2;
        _mocker.Use(_options);
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow_NoResponse()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, FileNames.RequestsFolderName);
        var responsesFolder = Path.Combine(StorageFolder, FileNames.ResponsesFolderName);
        var request = new RequestResultModel {CorrelationId = "bla123"};
        var millis = 1234;
        var requestFilePath = Path.Combine(requestsFolder, $"{millis}-{request.CorrelationId}.json");
        var responseFilePath = Path.Combine(responsesFolder, $"{request.CorrelationId}.json");
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var dateTimeMock = _mocker.GetMock<IDateTime>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        dateTimeMock
            .Setup(m => m.UtcNowUnix)
            .Returns(millis);

        // Act
        await source.AddRequestResultAsync(request, null, CancellationToken.None);

        // Assert
        fileServiceMock
            .Verify(m => m.WriteAllTextAsync(requestFilePath, It.Is<string>(c => c.Contains(request.CorrelationId)),
                It.IsAny<CancellationToken>()));
        fileServiceMock
            .Verify(m => m.WriteAllTextAsync(responseFilePath, It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        Assert.IsFalse(request.HasResponse);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow_WithResponse()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, FileNames.RequestsFolderName);
        var responsesFolder = Path.Combine(StorageFolder, FileNames.ResponsesFolderName);
        var request = new RequestResultModel {CorrelationId = "bla123"};
        var response = new ResponseModel {StatusCode = 200};
        var millis = 1234;
        var requestFilePath = Path.Combine(requestsFolder, $"{millis}-{request.CorrelationId}.json");
        var responseFilePath = Path.Combine(responsesFolder, $"{request.CorrelationId}.json");
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var dateTimeMock = _mocker.GetMock<IDateTime>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        dateTimeMock
            .Setup(m => m.UtcNowUnix)
            .Returns(millis);

        // Act
        await source.AddRequestResultAsync(request, response, CancellationToken.None);

        // Assert
        fileServiceMock
            .Verify(m => m.WriteAllTextAsync(requestFilePath, It.Is<string>(c => c.Contains(request.CorrelationId)),
                It.IsAny<CancellationToken>()));
        fileServiceMock
            .Verify(m => m.WriteAllTextAsync(responseFilePath, It.Is<string>(c => c.Contains("200")),
                It.IsAny<CancellationToken>()));
        Assert.IsTrue(request.HasResponse);
    }

    [TestMethod]
    public async Task AddStubAsync_HappyFlow()
    {
        // Arrange
        var stubsFolder = Path.Combine(StorageFolder, FileNames.StubsFolderName);
        var stub = new StubModel {Id = "situation-01"};
        var filePath = Path.Combine(stubsFolder, $"{stub.Id}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.WriteAllTextAsync(filePath, It.Is<string>(c => c.Contains(stub.Id)),
                It.IsAny<CancellationToken>()));

        // Act / assert
        await source.AddStubAsync(stub, CancellationToken.None);
        fileSystemStubCacheMock.Verify(m => m.AddOrReplaceStub(stub));
    }

    [TestMethod]
    public async Task GetRequestAsync_RequestNotFound_ShouldReturnNull()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var requestsFolder = Path.Combine(StorageFolder, FileNames.RequestsFolderName);
        var expectedPath = Path.Combine(requestsFolder, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await source.GetRequestAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetRequestAsync_RequestFound_ShouldReturnRequest()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var requestsFolder = Path.Combine(StorageFolder, FileNames.RequestsFolderName);
        var expectedPath = Path.Combine(requestsFolder, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = correlationId}));

        // Act
        var result = await source.GetRequestAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.AreEqual(correlationId, result.CorrelationId);
    }

    [TestMethod]
    public async Task GetResponseAsync_ResponseNotFound_ShouldReturnNull()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var responsesFolder = Path.Combine(StorageFolder, FileNames.ResponsesFolderName);
        var expectedPath = Path.Combine(responsesFolder, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await source.GetResponseAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetResponseAsync_ResponseFound_ShouldReturnResponse()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var responsesFolder = Path.Combine(StorageFolder, FileNames.ResponsesFolderName);
        var expectedPath = Path.Combine(responsesFolder, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(JsonConvert.SerializeObject(new ResponseModel {StatusCode = 200}));

        // Act
        var result = await source.GetResponseAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteAllRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, FileNames.RequestsFolderName);
        var responsesFolder = Path.Combine(StorageFolder, FileNames.ResponsesFolderName);
        var files = new[]
        {
            Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
        };

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.GetFilesAsync(requestsFolder, "*.json", It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        fileServiceMock
            .Setup(m => m.DeleteFileAsync(It.Is<string>(f => files.Contains(f)), It.IsAny<CancellationToken>()));

        var responsePath1 = Path.Combine(responsesFolder, "request-01.json");
        var responsePath2 = Path.Combine(responsesFolder, "request-02.json");

        fileServiceMock
            .Setup(m => m.FileExistsAsync(responsePath1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fileServiceMock
            .Setup(m => m.FileExistsAsync(responsePath2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await source.DeleteAllRequestResultsAsync(CancellationToken.None);

        // Assert
        fileServiceMock.Verify(m => m.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
        fileServiceMock.Verify(m => m.DeleteFileAsync(responsePath1, It.IsAny<CancellationToken>()), Times.Never);
        fileServiceMock.Verify(m => m.DeleteFileAsync(responsePath2, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestDoesntExist_ShouldReturnFalse()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var requestsPath =
            Path.Combine(_options.CurrentValue.Storage.FileStorageLocation, FileNames.RequestsFolderName);
        var expectedRequestPath = Path.Combine(requestsPath, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedRequestPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await source.DeleteRequestAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        fileServiceMock.Verify(m => m.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestExists_ResponseDoesntExist_ShouldDeleteFileAndReturnTrue()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var requestsPath =
            Path.Combine(_options.CurrentValue.Storage.FileStorageLocation, FileNames.RequestsFolderName);
        var responsesPath =
            Path.Combine(_options.CurrentValue.Storage.FileStorageLocation, FileNames.ResponsesFolderName);
        var expectedRequestPath = Path.Combine(requestsPath, $"{correlationId}.json");
        var expectedResponsePath = Path.Combine(responsesPath, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedRequestPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedResponsePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await source.DeleteRequestAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        fileServiceMock.Verify(m => m.DeleteFileAsync(expectedRequestPath, It.IsAny<CancellationToken>()));
        fileServiceMock.Verify(m => m.DeleteFileAsync(expectedResponsePath, It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestExists_ResponseExists_ShouldDeleteFileAndReturnTrue()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var requestsPath =
            Path.Combine(_options.CurrentValue.Storage.FileStorageLocation, FileNames.RequestsFolderName);
        var responsesPath =
            Path.Combine(_options.CurrentValue.Storage.FileStorageLocation, FileNames.ResponsesFolderName);
        var expectedRequestPath = Path.Combine(requestsPath, $"{correlationId}.json");
        var expectedResponsePath = Path.Combine(responsesPath, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedRequestPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedResponsePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await source.DeleteRequestAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        fileServiceMock.Verify(m => m.DeleteFileAsync(expectedRequestPath, It.IsAny<CancellationToken>()));
        fileServiceMock.Verify(m => m.DeleteFileAsync(expectedResponsePath, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task DeleteStubAsync_StubDoesntExist_ShouldReturnFalse()
    {
        // Arrange
        var stubsFolder = Path.Combine(StorageFolder, FileNames.StubsFolderName);
        const string stubId = "situation-01";
        var filePath = Path.Combine(stubsFolder, $"{stubId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(filePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await source.DeleteStubAsync(stubId, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        fileSystemStubCacheMock.Verify(m => m.DeleteStub(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task DeleteStubAsync_RequestExist_ShouldDeleteRequestAndReturnTrue()
    {
        // Arrange
        var stubsFolder = Path.Combine(StorageFolder, FileNames.StubsFolderName);
        const string stubId = "situation-01";
        var filePath = Path.Combine(stubsFolder, $"{stubId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(filePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await source.DeleteStubAsync(stubId, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        fileSystemStubCacheMock.Verify(m => m.DeleteStub(stubId));
    }

    [TestMethod]
    public async Task DeleteStubAsync_StubExists_ShouldReturnTrueAndDeleteStub()
    {
        // Arrange
        var stubsFolder = Path.Combine(StorageFolder, "stubs");
        const string stubId = "situation-01";
        var filePath = Path.Combine(stubsFolder, $"{stubId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(filePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fileServiceMock
            .Setup(m => m.DeleteFileAsync(filePath, It.IsAny<CancellationToken>()));

        // Act
        var result = await source.DeleteStubAsync(stubId, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        fileSystemStubCacheMock.Verify(m => m.DeleteStub(stubId));
    }

    [TestMethod]
    public async Task GetRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, FileNames.RequestsFolderName);
        var files = new[]
        {
            Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
        };

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.GetFilesAsync(requestsFolder, "*.json", It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        var requestFileContents = new[]
        {
            JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = "request-01"}),
            JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = "request-02"})
        };

        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var contents = requestFileContents[i];
            fileServiceMock
                .Setup(m => m.ReadAllTextAsync(file, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contents);
        }

        // Act
        var result = (await source.GetRequestResultsAsync(null, CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("request-02", result[0].CorrelationId);
        Assert.AreEqual("request-01", result[1].CorrelationId);
    }

    [TestMethod]
    public async Task CleanOldRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, FileNames.RequestsFolderName);
        var responsesFolder = Path.Combine(StorageFolder, FileNames.ResponsesFolderName);
        var files = new[]
        {
            Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json"),
            Path.Combine(requestsFolder, "request-03.json")
        };

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.GetFilesAsync(requestsFolder, "*.json", It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        var fileDateTimeMapping = new[]
        {
            (files[0], DateTime.Now.AddSeconds(-2)), (files[1], DateTime.Now.AddSeconds(-3)),
            (files[2], DateTime.Now.AddSeconds(-1))
        };
        foreach (var (path, lastWriteDateTime) in fileDateTimeMapping)
        {
            fileServiceMock
                .Setup(m => m.GetLastWriteTime(path))
                .Returns(lastWriteDateTime);
        }

        var responsePath2 = Path.Combine(responsesFolder, "request-02.json");
        fileServiceMock
            .Setup(m => m.FileExistsAsync(responsePath2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await source.CleanOldRequestResultsAsync(CancellationToken.None);

        // Assert
        fileServiceMock.Verify(m => m.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        fileServiceMock.Verify(m => m.DeleteFileAsync(files[1], It.IsAny<CancellationToken>()));
        fileServiceMock.Verify(m => m.DeleteFileAsync(responsePath2, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task GetStubsAsync_HappyFlow()
    {
        // Arrange
        var stubs = new[] {new StubModel {Id = "stub1"}};

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCacheAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        // Act
        var result = await source.GetStubsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(stubs, result);
    }

    [TestMethod]
    public async Task GetStubAsync_StubFound_ShouldReturnStub()
    {
        // Arrange
        var stubs = new[] {new StubModel {Id = "stub1"}, new StubModel {Id = "stub2"}};

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCacheAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        // Act
        var result = await source.GetStubAsync("stub2", CancellationToken.None);

        // Assert
        Assert.AreEqual(stubs[1], result);
    }

    [TestMethod]
    public async Task GetStubAsync_StubNotFound_ShouldReturnNull()
    {
        // Arrange
        var stubs = new[] {new StubModel {Id = "stub1"}, new StubModel {Id = "stub2"}};

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCacheAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        // Act
        var result = await source.GetStubAsync("stub3", CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetStubsOverviewAsync_HappyFlow()
    {
        // Arrange
        var stubs = new[]
        {
            new StubModel {Id = "stub1", Tenant = "tenant1", Enabled = true},
            new StubModel {Id = "stub2", Tenant = "tenant2", Enabled = false}
        };

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCacheAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        // Act
        var result = (await source.GetStubsOverviewAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.AreEqual(stubs[0].Id, result[0].Id);
        Assert.AreEqual(stubs[0].Tenant, result[0].Tenant);
        Assert.AreEqual(stubs[0].Enabled, result[0].Enabled);

        Assert.AreEqual(stubs[1].Id, result[1].Id);
        Assert.AreEqual(stubs[1].Tenant, result[1].Tenant);
        Assert.AreEqual(stubs[1].Enabled, result[1].Enabled);
    }

    [TestMethod]
    public async Task PrepareStubSourceAsync_HappyFlow()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        // Act
        await source.PrepareStubSourceAsync(CancellationToken.None);

        // Assert
        fileServiceMock.Verify(m => m.DirectoryExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(4));
        fileServiceMock.Verify(m => m.CreateDirectoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(4));
        fileSystemStubCacheMock.Verify(m => m.GetOrUpdateStubCacheAsync(It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task PrepareStubSourceAsync_FileStorageLocationNotSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _options.CurrentValue.Storage.FileStorageLocation = null;
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        // Act
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            source.PrepareStubSourceAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task FindRequestFilenameAsync_OldRequestPathFound()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        var expectedPath = Path.Join(StorageFolder, "requests", $"{correlationId}.json");
        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await source.FindRequestFilenameAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedPath, result);
    }

    [TestMethod]
    public async Task FindRequestFilenameAsync_NewRequestPathFound()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        var expectedPath = Path.Join(StorageFolder, "requests", $"{correlationId}.json");
        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var expectedFile = $"1-{correlationId}.json";
        var files = new[] {expectedFile};
        fileServiceMock
            .Setup(m => m.GetFilesAsync(Path.Join(StorageFolder, "requests"), $"*-{correlationId}.json",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        // Act
        var result = await source.FindRequestFilenameAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedFile, result);
    }

    [TestMethod]
    public async Task FindRequestFilenameAsync_NewRequestPathNotFound()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        var expectedPath = Path.Join(StorageFolder, "requests", $"{correlationId}.json");
        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        fileServiceMock
            .Setup(m => m.GetFilesAsync(Path.Join(StorageFolder, "requests"), $"*-{correlationId}.json",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        var result = await source.FindRequestFilenameAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }
}
