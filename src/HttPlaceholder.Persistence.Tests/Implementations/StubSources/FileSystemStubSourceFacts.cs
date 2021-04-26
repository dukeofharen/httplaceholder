using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem;
using HttPlaceholder.Persistence.Implementations.StubSources;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources
{
    [TestClass]
    public class FileSystemStubSourceFacts
    {
        private const string StorageFolder = @"C:\storage";
        private readonly IOptions<SettingsModel> _options = MockSettingsFactory.GetSettings();
        private readonly Mock<IFileService> _fileServiceMock = new();
        private readonly Mock<IFileSystemStubCache> _mockFileSystemStubCache = new();
        private FileSystemStubSource _source;

        [TestInitialize]
        public void Initialize()
        {
            _options.Value.Storage.FileStorageLocation = StorageFolder;
            _options.Value.Storage.OldRequestsQueueLength = 2;

            _source = new FileSystemStubSource(
                _fileServiceMock.Object,
                _options,
                _mockFileSystemStubCache.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _fileServiceMock.VerifyAll();
            _mockFileSystemStubCache.VerifyAll();
        }

        [TestMethod]
        public async Task AddRequestResultAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            var request = new RequestResultModel {CorrelationId = "bla123"};
            var filePath = Path.Combine(requestsFolder, $"{request.CorrelationId}.json");

            _fileServiceMock
                .Setup(m => m.WriteAllText(filePath, It.Is<string>(c => c.Contains(request.CorrelationId))));

            // act / assert
            await _source.AddRequestResultAsync(request);
        }

        [TestMethod]
        public async Task AddStubAsync_HappyFlow()
        {
            // arrange
            var stubsFolder = Path.Combine(StorageFolder, "stubs");
            var stub = new StubModel {Id = "situation-01"};
            var filePath = Path.Combine(stubsFolder, $"{stub.Id}.json");

            _fileServiceMock
                .Setup(m => m.WriteAllText(filePath, It.Is<string>(c => c.Contains(stub.Id))));

            // act / assert
            await _source.AddStubAsync(stub);
            _mockFileSystemStubCache.Verify(m => m.ClearStubCache());
        }

        [TestMethod]
        public async Task GetRequestAsync_RequestNotFound_ShouldReturnNull()
        {
            // Arrange
            var correlationId = Guid.NewGuid().ToString();
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            var expectedPath = Path.Combine(requestsFolder, $"{correlationId}.json");
            _fileServiceMock
                .Setup(m => m.FileExists(expectedPath))
                .Returns(false);

            // Act
            var result = await _source.GetRequestAsync(correlationId);

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
            _fileServiceMock
                .Setup(m => m.FileExists(expectedPath))
                .Returns(true);
            _fileServiceMock
                .Setup(m => m.ReadAllText(expectedPath))
                .Returns(JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = correlationId}));

            // Act
            var result = await _source.GetRequestAsync(correlationId);

            // Assert
            Assert.AreEqual(correlationId, result.CorrelationId);
        }

        [TestMethod]
        public async Task DeleteAllRequestResultsAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            var files = new[]
            {
                Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
            };

            _fileServiceMock
                .Setup(m => m.GetFiles(requestsFolder, "*.json"))
                .Returns(files);

            _fileServiceMock
                .Setup(m => m.DeleteFile(It.Is<string>(f => files.Contains(f))));

            // act / assert
            await _source.DeleteAllRequestResultsAsync();
        }

        [TestMethod]
        public async Task DeleteStubAsync_StubDoesntExist_ShouldReturnFalse()
        {
            // arrange
            var stubsFolder = Path.Combine(StorageFolder, "stubs");
            const string stubId = "situation-01";
            var filePath = Path.Combine(stubsFolder, $"{stubId}.json");
            _fileServiceMock
                .Setup(m => m.FileExists(filePath))
                .Returns(false);

            // act
            var result = await _source.DeleteStubAsync(stubId);

            // assert
            Assert.IsFalse(result);
            _mockFileSystemStubCache.Verify(m => m.ClearStubCache(), Times.Never);
        }

        [TestMethod]
        public async Task DeleteStubAsync_StubExists_ShouldReturnTrueAndDeleteStub()
        {
            // arrange
            var stubsFolder = Path.Combine(StorageFolder, "stubs");
            var stubId = "situation-01";
            var filePath = Path.Combine(stubsFolder, $"{stubId}.json");
            _fileServiceMock
                .Setup(m => m.FileExists(filePath))
                .Returns(true);
            _fileServiceMock
                .Setup(m => m.DeleteFile(filePath));

            // act
            var result = await _source.DeleteStubAsync(stubId);

            // assert
            Assert.IsTrue(result);
            _mockFileSystemStubCache.Verify(m => m.ClearStubCache());
        }

        [TestMethod]
        public async Task GetRequestResultsAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            var files = new[]
            {
                Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
            };

            _fileServiceMock
                .Setup(m => m.GetFiles(requestsFolder, "*.json"))
                .Returns(files);

            var requestFileContents = new[]
            {
                JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = "request-01"}),
                JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = "request-02"})
            };

            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var contents = requestFileContents[i];
                _fileServiceMock
                    .Setup(m => m.ReadAllText(file))
                    .Returns(contents);
            }

            // act
            var result = (await _source.GetRequestResultsAsync()).ToArray();

            // assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("request-01", result[0].CorrelationId);
            Assert.AreEqual("request-02", result[1].CorrelationId);
        }

        [TestMethod]
        public async Task GetRequestResultsOverviewAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            var files = new[]
            {
                Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json")
            };

            _fileServiceMock
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
                _fileServiceMock
                    .Setup(m => m.ReadAllText(file))
                    .Returns(contents);
            }

            // act
            var result = (await _source.GetRequestResultsOverviewAsync()).ToArray();

            // assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("request-01", result[0].CorrelationId);
            Assert.AreEqual("request-02", result[1].CorrelationId);
        }

        [TestMethod]
        public async Task CleanOldRequestResultsAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            var files = new[]
            {
                Path.Combine(requestsFolder, "request-01.json"), Path.Combine(requestsFolder, "request-02.json"),
                Path.Combine(requestsFolder, "request-03.json")
            };

            _fileServiceMock
                .Setup(m => m.GetFiles(requestsFolder, "*.json"))
                .Returns(files);

            var requestFileContents = new[]
            {
                JsonConvert.SerializeObject(new RequestResultModel
                {
                    CorrelationId = "request-01", RequestEndTime = DateTime.Now.AddHours(-2)
                }),
                JsonConvert.SerializeObject(new RequestResultModel
                {
                    CorrelationId = "request-02", RequestEndTime = DateTime.Now.AddHours(-1)
                }),
                JsonConvert.SerializeObject(new RequestResultModel
                {
                    CorrelationId = "request-03", RequestEndTime = DateTime.Now.AddMinutes(-30)
                })
            };

            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var contents = requestFileContents[i];
                _fileServiceMock
                    .Setup(m => m.ReadAllText(file))
                    .Returns(contents);
            }

            _fileServiceMock
                .Setup(m => m.DeleteFile(files[0]));

            // act
            await _source.CleanOldRequestResultsAsync();

            // assert
            _fileServiceMock
                .Verify(m => m.DeleteFile(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetStubsAsync_HappyFlow()
        {
            // Arrange
            var stubs = new[] {new StubModel {Id = "stub1"}};
            _mockFileSystemStubCache
                .Setup(m => m.GetOrUpdateStubCache())
                .Returns(stubs);

            // Act
            var result = await _source.GetStubsAsync();

            // Assert
            Assert.AreEqual(stubs, result);
        }

        [TestMethod]
        public async Task GetStubAsync_StubFound_ShouldReturnStub()
        {
            // Arrange
            var stubs = new[]
            {
                new StubModel {Id = "stub1"},
                new StubModel {Id = "stub2"}
            };
            _mockFileSystemStubCache
                .Setup(m => m.GetOrUpdateStubCache())
                .Returns(stubs);

            // Act
            var result = await _source.GetStubAsync("stub2");

            // Assert
            Assert.AreEqual(stubs[1], result);
        }

        [TestMethod]
        public async Task GetStubAsync_StubNotFound_ShouldReturnNull()
        {
            // Arrange
            var stubs = new[]
            {
                new StubModel {Id = "stub1"},
                new StubModel {Id = "stub2"}
            };
            _mockFileSystemStubCache
                .Setup(m => m.GetOrUpdateStubCache())
                .Returns(stubs);

            // Act
            var result = await _source.GetStubAsync("stub3");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetStubsOverviewAsync_HappyFlow()
        {
            // Arrange
            var stubs = new[]
            {
                new StubModel
                {
                    Id = "stub1",
                    Tenant = "tenant1",
                    Enabled = true
                },
                new StubModel
                {
                    Id = "stub2",
                    Tenant = "tenant2",
                    Enabled = false
                }
            };
            _mockFileSystemStubCache
                .Setup(m => m.GetOrUpdateStubCache())
                .Returns(stubs);

            // Act
            var result = (await _source.GetStubsOverviewAsync()).ToArray();

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
            // act
            await _source.PrepareStubSourceAsync();

            // assert
            _fileServiceMock.Verify(m => m.DirectoryExists(It.IsAny<string>()), Times.Exactly(2));
            _fileServiceMock.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Exactly(2));
            _mockFileSystemStubCache.Verify(m => m.GetOrUpdateStubCache());
        }
    }
}
