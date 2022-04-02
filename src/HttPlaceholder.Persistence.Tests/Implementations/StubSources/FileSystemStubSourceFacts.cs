using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem;
using HttPlaceholder.Persistence.Implementations.StubSources;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class FileSystemStubSourceFacts
{
    private const string StorageFolder = @"C:\storage";

    private readonly AutoMocker _mocker = new();
    private readonly IOptions<SettingsModel> _options = MockSettingsFactory.GetSettings();

    [TestInitialize]
    public void Initialize()
    {
        _options.Value.Storage.FileStorageLocation = StorageFolder;
        _options.Value.Storage.OldRequestsQueueLength = 2;
        _mocker.Use(_options);
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, "requests");
        var request = new RequestResultModel { CorrelationId = "bla123" };
        var filePath = Path.Combine(requestsFolder, $"{request.CorrelationId}.json");
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.WriteAllText(filePath, It.Is<string>(c => c.Contains(request.CorrelationId))));

        // Act / assert
        await source.AddRequestResultAsync(request, null); // TODO
    }

    [TestMethod]
    public async Task AddStubAsync_HappyFlow()
    {
        // Arrange
        var stubsFolder = Path.Combine(StorageFolder, "stubs");
        var stub = new StubModel { Id = "situation-01" };
        var filePath = Path.Combine(stubsFolder, $"{stub.Id}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.WriteAllText(filePath, It.Is<string>(c => c.Contains(stub.Id))));

        // Act / assert
        await source.AddStubAsync(stub);
        fileSystemStubCacheMock.Verify(m => m.ClearStubCache());
    }

    [TestMethod]
    public async Task GetRequestAsync_RequestNotFound_ShouldReturnNull()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var requestsFolder = Path.Combine(StorageFolder, "requests");
        var expectedPath = Path.Combine(requestsFolder, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExists(expectedPath))
            .Returns(false);

        // Act
        var result = await source.GetRequestAsync(correlationId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetRequestAsync_RequestFound_ShouldReturnRequest()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var requestsFolder = Path.Combine(StorageFolder, "requests");
        var expectedPath = Path.Combine(requestsFolder, $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExists(expectedPath))
            .Returns(true);
        fileServiceMock
            .Setup(m => m.ReadAllText(expectedPath))
            .Returns(JsonConvert.SerializeObject(new RequestResultModel { CorrelationId = correlationId }));

        // Act
        var result = await source.GetRequestAsync(correlationId);

        // Assert
        Assert.AreEqual(correlationId, result.CorrelationId);
    }

    [TestMethod]
    public async Task DeleteAllRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, "requests");
        var files = new[]
        {
            Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
        };

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.GetFiles(requestsFolder, "*.json"))
            .Returns(files);

        fileServiceMock
            .Setup(m => m.DeleteFile(It.Is<string>(f => files.Contains(f))));

        // Act / assert
        await source.DeleteAllRequestResultsAsync();
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestDoesntExist_ShouldReturnFalse()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var expectedPath = Path.Combine(
            _options.Value.Storage.FileStorageLocation,
            Constants.RequestsFolderName,
            $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExists(expectedPath))
            .Returns(false);

        // Act
        var result = await source.DeleteRequestAsync(correlationId);

        // Assert
        Assert.IsFalse(result);
        fileServiceMock.Verify(m => m.DeleteFile(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestExists_ShouldDeleteFileAndReturnTrue()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var expectedPath = Path.Combine(
            _options.Value.Storage.FileStorageLocation,
            Constants.RequestsFolderName,
            $"{correlationId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExists(expectedPath))
            .Returns(true);

        // Act
        var result = await source.DeleteRequestAsync(correlationId);

        // Assert
        Assert.IsTrue(result);
        fileServiceMock.Verify(m => m.DeleteFile(expectedPath));
    }

    [TestMethod]
    public async Task DeleteStubAsync_StubDoesntExist_ShouldReturnFalse()
    {
        // Arrange
        var stubsFolder = Path.Combine(StorageFolder, "stubs");
        const string stubId = "situation-01";
        var filePath = Path.Combine(stubsFolder, $"{stubId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExists(filePath))
            .Returns(false);

        // Act
        var result = await source.DeleteStubAsync(stubId);

        // Assert
        Assert.IsFalse(result);
        fileSystemStubCacheMock.Verify(m => m.ClearStubCache(), Times.Never);
    }

    [TestMethod]
    public async Task DeleteStubAsync_RequestExist_ShouldDeleteRequestAndReturnTrue()
    {
        // Arrange
        var stubsFolder = Path.Combine(StorageFolder, "stubs");
        const string stubId = "situation-01";
        var filePath = Path.Combine(stubsFolder, $"{stubId}.json");

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.FileExists(filePath))
            .Returns(true);

        // Act
        var result = await source.DeleteStubAsync(stubId);

        // Assert
        Assert.IsTrue(result);
        fileSystemStubCacheMock.Verify(m => m.ClearStubCache());
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
            .Setup(m => m.FileExists(filePath))
            .Returns(true);
        fileServiceMock
            .Setup(m => m.DeleteFile(filePath));

        // Act
        var result = await source.DeleteStubAsync(stubId);

        // Assert
        Assert.IsTrue(result);
        fileSystemStubCacheMock.Verify(m => m.ClearStubCache());
    }

    [TestMethod]
    public async Task GetRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, "requests");
        var files = new[]
        {
            Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
        };

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.GetFiles(requestsFolder, "*.json"))
            .Returns(files);

        var requestFileContents = new[]
        {
            JsonConvert.SerializeObject(new RequestResultModel { CorrelationId = "request-01" }),
            JsonConvert.SerializeObject(new RequestResultModel { CorrelationId = "request-02" })
        };

        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var contents = requestFileContents[i];
            fileServiceMock
                .Setup(m => m.ReadAllText(file))
                .Returns(contents);
        }

        // Act
        var result = (await source.GetRequestResultsAsync()).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("request-01", result[0].CorrelationId);
        Assert.AreEqual("request-02", result[1].CorrelationId);
    }

    [TestMethod]
    public async Task GetRequestResultsOverviewAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, "requests");
        var files = new[]
        {
            Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
        };

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.GetFiles(requestsFolder, "*.json"))
            .Returns(files);

        var requestFileContents = new[]
        {
            JsonConvert.SerializeObject(new RequestResultModel
            {
                CorrelationId = "request-01", RequestParameters = new RequestParametersModel()
            }),
            JsonConvert.SerializeObject(new RequestResultModel
            {
                CorrelationId = "request-02", RequestParameters = new RequestParametersModel()
            })
        };

        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var contents = requestFileContents[i];
            fileServiceMock
                .Setup(m => m.ReadAllText(file))
                .Returns(contents);
        }

        // Act
        var result = (await source.GetRequestResultsOverviewAsync()).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("request-01", result[0].CorrelationId);
        Assert.AreEqual("request-02", result[1].CorrelationId);
    }

    [TestMethod]
    public async Task CleanOldRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var requestsFolder = Path.Combine(StorageFolder, "requests");
        var files = new[]
        {
            Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json"),
            Path.Combine(requestsFolder, "request-03.json")
        };

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileServiceMock
            .Setup(m => m.GetFiles(requestsFolder, "*.json"))
            .Returns(files);

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

        // Act
        await source.CleanOldRequestResultsAsync();

        // Assert
        fileServiceMock.Verify(m => m.DeleteFile(It.IsAny<string>()), Times.Once);
        fileServiceMock.Verify(m => m.DeleteFile(files[1]));
    }

    // [TestMethod]
    // public async Task CleanOldRequestResultsAsync_HappyFlow()
    // {
    //     // Arrange
    //     var requestsFolder = Path.Combine(StorageFolder, "requests");
    //     var files = new[]
    //     {
    //         Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json"),
    //         Path.Combine(requestsFolder, "request-03.json")
    //     };
    //
    //     _fileServiceMock
    //         .Setup(m => m.GetFiles(requestsFolder, "*.json"))
    //         .Returns(files);
    //
    //     var requestFileContents = new[]
    //     {
    //         JsonConvert.SerializeObject(new RequestResultModel
    //         {
    //             CorrelationId = "request-01", RequestEndTime = DateTime.Now.AddHours(-2)
    //         }),
    //         JsonConvert.SerializeObject(new RequestResultModel
    //         {
    //             CorrelationId = "request-02", RequestEndTime = DateTime.Now.AddHours(-1)
    //         }),
    //         JsonConvert.SerializeObject(new RequestResultModel
    //         {
    //             CorrelationId = "request-03", RequestEndTime = DateTime.Now.AddMinutes(-30)
    //         })
    //     };
    //
    //     for (var i = 0; i < files.Length; i++)
    //     {
    //         var file = files[i];
    //         var contents = requestFileContents[i];
    //         _fileServiceMock
    //             .Setup(m => m.ReadAllText(file))
    //             .Returns(contents);
    //     }
    //
    //     _fileServiceMock
    //         .Setup(m => m.DeleteFile(files[0]));
    //
    //     // Act
    //     await _source.CleanOldRequestResultsAsync();
    //
    //     // Assert
    //     _fileServiceMock
    //         .Verify(m => m.DeleteFile(It.IsAny<string>()), Times.Once);
    // }

    [TestMethod]
    public async Task GetStubsAsync_HappyFlow()
    {
        // Arrange
        var stubs = new[] { new StubModel { Id = "stub1" } };

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCache())
            .Returns(stubs);

        // Act
        var result = await source.GetStubsAsync();

        // Assert
        Assert.AreEqual(stubs, result);
    }

    [TestMethod]
    public async Task GetStubAsync_StubFound_ShouldReturnStub()
    {
        // Arrange
        var stubs = new[] { new StubModel { Id = "stub1" }, new StubModel { Id = "stub2" } };

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCache())
            .Returns(stubs);

        // Act
        var result = await source.GetStubAsync("stub2");

        // Assert
        Assert.AreEqual(stubs[1], result);
    }

    [TestMethod]
    public async Task GetStubAsync_StubNotFound_ShouldReturnNull()
    {
        // Arrange
        var stubs = new[] { new StubModel { Id = "stub1" }, new StubModel { Id = "stub2" } };

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCache())
            .Returns(stubs);

        // Act
        var result = await source.GetStubAsync("stub3");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetStubsOverviewAsync_HappyFlow()
    {
        // Arrange
        var stubs = new[]
        {
            new StubModel { Id = "stub1", Tenant = "tenant1", Enabled = true },
            new StubModel { Id = "stub2", Tenant = "tenant2", Enabled = false }
        };

        var fileSystemStubCacheMock = _mocker.GetMock<IFileSystemStubCache>();
        var source = _mocker.CreateInstance<FileSystemStubSource>();

        fileSystemStubCacheMock
            .Setup(m => m.GetOrUpdateStubCache())
            .Returns(stubs);

        // Act
        var result = (await source.GetStubsOverviewAsync()).ToArray();

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
        await source.PrepareStubSourceAsync();

        // Assert
        fileServiceMock.Verify(m => m.DirectoryExists(It.IsAny<string>()), Times.Exactly(2));
        fileServiceMock.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Exactly(2));
        fileSystemStubCacheMock.Verify(m => m.GetOrUpdateStubCache());
    }
}
