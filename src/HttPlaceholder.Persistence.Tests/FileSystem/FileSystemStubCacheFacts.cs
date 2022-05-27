using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem.Implementations;
using HttPlaceholder.Persistence.FileSystem.Models;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Tests.FileSystem;

[TestClass]
public class FileSystemStubCacheFacts
{
    private const string FileStorageLocation = "/etc/httplaceholder";
    private readonly AutoMocker _mocker = new();
    private readonly SettingsModel _settings = MockSettingsFactory.GetSettings();

    [TestInitialize]
    public void Initialize()
    {
        _settings.Storage.FileStorageLocation = FileStorageLocation;
        _mocker.Use(Options.Create(_settings));
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void EnsureAndGetMetadata_MetadataDoesntExistYet_ShouldCreateMetadata()
    {
        // Arrange
        var expectedPath = Path.Join(_settings.Storage.FileStorageLocation, Constants.MetadataFileName);

        var mockFileService = _mocker.GetMock<IFileService>();
        var cache = _mocker.CreateInstance<FileSystemStubCache>();

        mockFileService
            .Setup(m => m.FileExists(expectedPath))
            .Returns(false);

        string capturedMetadata = null;
        mockFileService
            .Setup(m => m.WriteAllText(expectedPath, It.IsAny<string>()))
            .Callback<string, string>((_, metadata) => capturedMetadata = metadata);

        // Act
        var result = cache.EnsureAndGetMetadata();

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

        var mockFileService = _mocker.GetMock<IFileService>();
        var cache = _mocker.CreateInstance<FileSystemStubCache>();

        var expectedPath = Path.Join(_settings.Storage.FileStorageLocation, Constants.MetadataFileName);
        mockFileService
            .Setup(m => m.FileExists(expectedPath))
            .Returns(true);

        var metadataContents = @$"{{""StubUpdateTrackingId"": ""{trackingId}""}}";
        mockFileService
            .Setup(m => m.ReadAllText(expectedPath))
            .Returns(metadataContents);

        // Act
        var result = cache.EnsureAndGetMetadata();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(trackingId, result.StubUpdateTrackingId);
    }

    [TestMethod]
    public void GetOrUpdateStubCache_StubCacheIsNull_ShouldInitializeCache()
    {
        // Arrange
        var trackingId = Guid.NewGuid().ToString();
        SetupMetadata(trackingId);
        SetupStubs();

        var cache = _mocker.CreateInstance<FileSystemStubCache>();

        // Act
        var result = cache.GetOrUpdateStubCache();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.AreEqual(trackingId, cache.StubUpdateTrackingId);
    }

    [TestMethod]
    public void GetOrUpdateStubCache_TrackingIdHasChanged_ShouldInitializeCache()
    {
        // Arrange
        var trackingId = Guid.NewGuid().ToString();
        SetupMetadata(trackingId);
        SetupStubs();

        var cache = _mocker.CreateInstance<FileSystemStubCache>();

        cache.StubUpdateTrackingId = Guid.NewGuid().ToString();

        // Act
        var result = cache.GetOrUpdateStubCache();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.AreEqual(trackingId, cache.StubUpdateTrackingId);
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

        var cache = _mocker.CreateInstance<FileSystemStubCache>();

        // Act
        var result = cache.GetOrUpdateStubCache().ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.IsTrue(result.All(s => s.Id == stub1.Id || s.Id == stub2.Id));
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

        var cache = _mocker.CreateInstance<FileSystemStubCache>();

        // Act / Assert
        Assert.IsTrue(
            cache.GetOrUpdateStubCache().SequenceEqual(cache.GetOrUpdateStubCache()));
    }

    private void SetupMetadata(string trackingId)
    {
        var expectedPath = Path.Join(_settings.Storage.FileStorageLocation, Constants.MetadataFileName);

        var mockFileService = _mocker.GetMock<IFileService>();

        mockFileService
            .Setup(m => m.FileExists(expectedPath))
            .Returns(true);

        var metadataContents = @$"{{""StubUpdateTrackingId"": ""{trackingId}""}}";
        mockFileService
            .Setup(m => m.ReadAllText(expectedPath))
            .Returns(metadataContents);
    }

    private void SetupStubs(params StubModel[] stubs)
    {
        var expectedPath = Path.Combine(_settings.Storage?.FileStorageLocation, Constants.StubsFolderName);

        var mockFileService = _mocker.GetMock<IFileService>();

        var counter = 0;
        var filePaths = new List<string>();
        foreach (var stub in stubs)
        {
            var filePath = $"stub{counter}.json";
            filePaths.Add(filePath);
            mockFileService
                .Setup(m => m.ReadAllText(filePath))
                .Returns(JsonConvert.SerializeObject(stub));
            counter++;
        }

        mockFileService
            .Setup(m => m.GetFiles(expectedPath, "*.json"))
            .Returns(filePaths.ToArray());
    }
}
