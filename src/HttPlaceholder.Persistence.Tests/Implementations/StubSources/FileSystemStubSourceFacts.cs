using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
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
        private Mock<IFileService> _fileServiceMock = new Mock<IFileService>();
        private FileSystemStubSource _source;

        [TestInitialize]
        public void Initialize()
        {
            _options.Value.Storage.FileStorageLocation = StorageFolder;
            _options.Value.Storage.OldRequestsQueueLength = 2;

            _source = new FileSystemStubSource(
                _fileServiceMock.Object,
                _options);
        }

        [TestCleanup]
        public void Cleanup() => _fileServiceMock.VerifyAll();

        [TestMethod]
        public async Task FileSystemStubSource_AddRequestResultAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(requestsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(requestsFolder));

            var request = new RequestResultModel
            {
                CorrelationId = "bla123"
            };
            var filePath = Path.Combine(requestsFolder, $"{request.CorrelationId}.json");

            _fileServiceMock
               .Setup(m => m.WriteAllText(filePath, It.Is<string>(c => c.Contains(request.CorrelationId))));

            // act / assert
            await _source.AddRequestResultAsync(request);
        }

        [TestMethod]
        public async Task FileSystemStubSource_AddStubAsync_HappyFlow()
        {
            // arrange
            var stubsFolder = Path.Combine(StorageFolder, "stubs");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(stubsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(stubsFolder));

            var stub = new StubModel
            {
                Id = "situation-01"
            };
            var filePath = Path.Combine(stubsFolder, $"{stub.Id}.json");

            _fileServiceMock
               .Setup(m => m.WriteAllText(filePath, It.Is<string>(c => c.Contains(stub.Id))));

            // act / assert
            await _source.AddStubAsync(stub);
        }

        [TestMethod]
        public async Task FileSystemStubSource_DeleteAllRequestResultsAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(requestsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(requestsFolder));

            var files = new[]
            {
            Path.Combine(requestsFolder, "request-01.json"),
            Path.Combine(requestsFolder, "request-02.json")
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
        public async Task FileSystemStubSource_DeleteStubAsync_StubDoesntExist_ShouldReturnFalse()
        {
            // arrange
            var stubsFolder = Path.Combine(StorageFolder, "stubs");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(stubsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(stubsFolder));

            var stubId = "situation-01";
            var filePath = Path.Combine(stubsFolder, $"{stubId}.json");
            _fileServiceMock
               .Setup(m => m.FileExists(filePath))
               .Returns(false);

            // act
            var result = await _source.DeleteStubAsync(stubId);

            // assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task FileSystemStubSource_DeleteStubAsync_StubExists_ShouldReturnTrueAndDeleteStub()
        {
            // arrange
            var stubsFolder = Path.Combine(StorageFolder, "stubs");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(stubsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(stubsFolder));

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
        }

        [TestMethod]
        public async Task FileSystemStubSource_GetRequestResultsAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(requestsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(requestsFolder));

            var files = new[]
            {
            Path.Combine(requestsFolder, "request-01.json"),
            Path.Combine(requestsFolder, "request-02.json")
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
        public async Task FileSystemStubSource_GetStubsAsync_HappyFlow()
        {
            // arrange
            var stubsFolder = Path.Combine(StorageFolder, "stubs");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(stubsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(stubsFolder));

            var files = new[]
            {
            Path.Combine(stubsFolder, "stub-01.json"),
            Path.Combine(stubsFolder, "stub-02.json")
         };

            _fileServiceMock
               .Setup(m => m.GetFiles(stubsFolder, "*.json"))
               .Returns(files);

            var stubFileContents = new[]
            {
            JsonConvert.SerializeObject(new StubModel {Id = "stub-01"}),
            JsonConvert.SerializeObject(new StubModel {Id = "stub-02"})
         };

            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var contents = stubFileContents[i];
                _fileServiceMock
                   .Setup(m => m.ReadAllText(file))
                   .Returns(contents);
            }

            // act
            var result = (await _source.GetStubsAsync()).ToArray();

            // assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("stub-01", result[0].Id);
            Assert.AreEqual("stub-02", result[1].Id);
        }

        [TestMethod]
        public async Task FileSystemStubSource_CleanOldRequestResultsAsync_HappyFlow()
        {
            // arrange
            var requestsFolder = Path.Combine(StorageFolder, "requests");
            _fileServiceMock
               .Setup(m => m.DirectoryExists(requestsFolder))
               .Returns(false);
            _fileServiceMock
               .Setup(m => m.CreateDirectory(requestsFolder));

            var files = new[]
            {
            Path.Combine(requestsFolder, "request-01.json"),
            Path.Combine(requestsFolder, "request-02.json"),
            Path.Combine(requestsFolder, "request-03.json")
         };

            _fileServiceMock
               .Setup(m => m.GetFiles(requestsFolder, "*.json"))
               .Returns(files);

            var requestFileContents = new[]
            {
            JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = "request-01", RequestEndTime = DateTime.Now.AddHours(-2)}),
            JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = "request-02", RequestEndTime = DateTime.Now.AddHours(-1)}),
            JsonConvert.SerializeObject(new RequestResultModel {CorrelationId = "request-03", RequestEndTime = DateTime.Now.AddMinutes(-30)}),
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
        public async Task FileSystemStubSource_PrepareStubSourceAsync_HappyFlow()
        {
            // act
            await _source.PrepareStubSourceAsync();

            // assert
            _fileServiceMock.Verify(m => m.DirectoryExists(It.IsAny<string>()), Times.Exactly(2));
            _fileServiceMock.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
