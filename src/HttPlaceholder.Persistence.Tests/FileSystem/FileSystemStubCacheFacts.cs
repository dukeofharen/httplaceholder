using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttPlaceholder.Common;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem.Implementations;
using HttPlaceholder.Persistence.FileSystem.Models;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Tests.FileSystem
{
    [TestClass]
    public class FileSystemStubCacheFacts
    {
        private const string FileStorageLocation = "/etc/httplaceholder";
        private readonly Mock<ILogger<FileSystemStubCache>> _mockLogger = new();
        private readonly Mock<IFileService> _mockFileService = new();
        private readonly IOptions<SettingsModel> _options = MockSettingsFactory.GetSettings();
        private FileSystemStubCache _cache;

        [TestInitialize]
        public void Initialize()
        {
            _options.Value.Storage.FileStorageLocation = FileStorageLocation;
            _cache = new FileSystemStubCache(
                _mockLogger.Object,
                _mockFileService.Object,
                _options);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _mockLogger.VerifyAll();
            _mockFileService.VerifyAll();
        }

        [TestMethod]
        public void EnsureAndGetMetadata_MetadataDoesntExistYet_ShouldCreateMetadata()
        {
            // Arrange
            var expectedPath = Path.Join(_options.Value.Storage.FileStorageLocation, Constants.MetadataFileName);
            _mockFileService
                .Setup(m => m.FileExists(expectedPath))
                .Returns(false);

            string capturedMetadata = null;
            _mockFileService
                .Setup(m => m.WriteAllText(expectedPath, It.IsAny<string>()))
                .Callback<string, string>((_, metadata) => capturedMetadata = metadata);

            // Act
            var result = _cache.EnsureAndGetMetadata();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.StubUpdateTrackingId);

            Assert.IsNotNull(capturedMetadata);

            var parsedCapturedMetadata = JsonConvert.DeserializeObject<FileStorageMetadataModel>(capturedMetadata);
            Assert.AreEqual(result.StubUpdateTrackingId, parsedCapturedMetadata.StubUpdateTrackingId);
        }

        [TestMethod]
        public void EnsureAndGetMetadata_MetadataExists_ShouldParseAndReturnMetadata()
        {
            // Arrange
            var trackingId = Guid.NewGuid().ToString();

            var expectedPath = Path.Join(_options.Value.Storage.FileStorageLocation, Constants.MetadataFileName);
            _mockFileService
                .Setup(m => m.FileExists(expectedPath))
                .Returns(true);

            var metadataContents = @$"{{""StubUpdateTrackingId"": ""{trackingId}""}}";
            _mockFileService
                .Setup(m => m.ReadAllText(expectedPath))
                .Returns(metadataContents);

            // Act
            var result = _cache.EnsureAndGetMetadata();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(trackingId, result.StubUpdateTrackingId);
        }

        [TestMethod]
        public void ClearStubCache_ShouldClearStubCacheAndTrackingId()
        {
            // Arrange
            var trackingId = Guid.NewGuid().ToString();
            SetupMetadata(trackingId);

            _cache.StubCache = new List<StubModel>();
            _cache.StubUpdateTrackingId = trackingId;

            // Act
            _cache.ClearStubCache();

            // Assert
            Assert.IsNull(_cache.StubCache);
            Assert.IsNotNull(_cache.StubUpdateTrackingId);
            Assert.AreNotEqual(trackingId, _cache.StubUpdateTrackingId);
            _mockFileService.Verify(m => m.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void GetOrUpdateStubCache_StubCacheIsNull_ShouldInitializeCache()
        {
            // Arrange
            var trackingId = Guid.NewGuid().ToString();
            SetupMetadata(trackingId);
            SetupStubs();

            // Act
            var result = _cache.GetOrUpdateStubCache();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(_cache.StubCache);
            Assert.AreEqual(trackingId, _cache.StubUpdateTrackingId);
        }

        [TestMethod]
        public void GetOrUpdateStubCache_TrackingIdHasChanged_ShouldInitializeCache()
        {
            // Arrange
            var trackingId = Guid.NewGuid().ToString();
            SetupMetadata(trackingId);
            SetupStubs();

            _cache.StubCache = new List<StubModel>();
            _cache.StubUpdateTrackingId = Guid.NewGuid().ToString();

            // Act
            var result = _cache.GetOrUpdateStubCache();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(_cache.StubCache);
            Assert.AreEqual(trackingId, _cache.StubUpdateTrackingId);
        }

        [TestMethod]
        public void GetOrUpdateStubCache_CheckDeserializationOfStubs()
        {
            // Arrange
            var trackingId = Guid.NewGuid().ToString();
            SetupMetadata(trackingId);

            var stub1 = new StubModel {Id = "stub1"};
            var stub2 = new StubModel {Id = "stub2"};
            SetupStubs(stub1, stub2);

            // Act
            var result = _cache.GetOrUpdateStubCache().ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual(stub1.Id, result[0].Id);
            Assert.AreEqual(stub2.Id, result[1].Id);
        }

        [TestMethod]
        public void GetOrUpdateStubCache_CallTwice_ShouldReturnSameStubCache()
        {
            // Arrange
            var trackingId = Guid.NewGuid().ToString();
            SetupMetadata(trackingId);

            var stub1 = new StubModel {Id = "stub1"};
            var stub2 = new StubModel {Id = "stub2"};
            SetupStubs(stub1, stub2);

            // Act / Assert
            Assert.AreEqual(
                _cache.GetOrUpdateStubCache(),
                _cache.GetOrUpdateStubCache());
        }

        private void SetupMetadata(string trackingId)
        {
            var expectedPath = Path.Join(_options.Value.Storage.FileStorageLocation, Constants.MetadataFileName);
            _mockFileService
                .Setup(m => m.FileExists(expectedPath))
                .Returns(true);

            var metadataContents = @$"{{""StubUpdateTrackingId"": ""{trackingId}""}}";
            _mockFileService
                .Setup(m => m.ReadAllText(expectedPath))
                .Returns(metadataContents);
        }

        private void SetupStubs(params StubModel[] stubs)
        {
            var expectedPath = Path.Combine(_options.Value.Storage?.FileStorageLocation, Constants.StubsFolderName);

            var counter = 0;
            var filePaths = new List<string>();
            foreach (var stub in stubs)
            {
                var filePath = $"stub{counter}.json";
                filePaths.Add(filePath);
                _mockFileService
                    .Setup(m => m.ReadAllText(filePath))
                    .Returns(JsonConvert.SerializeObject(stub));
                counter++;
            }

            _mockFileService
                .Setup(m => m.GetFiles(expectedPath, "*.json"))
                .Returns(filePaths.ToArray());
        }
    }
}
