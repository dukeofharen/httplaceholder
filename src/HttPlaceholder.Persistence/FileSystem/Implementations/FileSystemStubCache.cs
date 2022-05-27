using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.FileSystem.Implementations;

/// <inheritdoc />
internal class FileSystemStubCache : IFileSystemStubCache
{
    private static readonly object _cacheUpdateLock = new();
    internal string StubUpdateTrackingId;
    internal readonly ConcurrentDictionary<string, StubModel> StubCache = new();

    private readonly ILogger<FileSystemStubCache> _logger;
    private readonly IFileService _fileService;
    private readonly SettingsModel _settings;

    public FileSystemStubCache(
        ILogger<FileSystemStubCache> logger,
        IFileService fileService,
        IOptions<SettingsModel> options)
    {
        _logger = logger;
        _fileService = fileService;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public IEnumerable<StubModel> GetOrUpdateStubCache()
    {
        var shouldUpdateCache = false;

        // Check if the "stub update tracking ID" variable has a new value.
        var metadata = EnsureAndGetMetadata();
        if (StubUpdateTrackingId == null)
        {
            // The local cache hasn't been initialized yet. Do that now.
            _logger.LogInformation(
                "Initializing the cache, because either the local stub cache or tracking ID is not set yet.");
            UpdateLocalStubUpdateTrackingId(metadata?.StubUpdateTrackingId ?? Guid.NewGuid().ToString());
            shouldUpdateCache = true;
        }
        else if (StubUpdateTrackingId != metadata.StubUpdateTrackingId)
        {
            // ID has been changed. Update the stub cache.
            _logger.LogInformation("Initializing the cache, because the tracking ID on disk has been changed.");
            UpdateLocalStubUpdateTrackingId(metadata.StubUpdateTrackingId);
            shouldUpdateCache = true;
        }

        if (shouldUpdateCache)
        {
            var path = GetStubsFolder();
            var files = _fileService.GetFiles(path, "*.json");
            StubCache.Clear();
            var newCache = files
                .Select(filePath => _fileService
                    .ReadAllText(filePath))
                .Select(JsonConvert.DeserializeObject<StubModel>);
            foreach (var item in newCache)
            {
                if (!StubCache.TryAdd(item.Id, item))
                {
                    _logger.LogWarning("Could not add stub with ID '{}' to cache.", item.Id);
                }
            }
        }

        return StubCache.Values;
    }

    /// <inheritdoc />
    public void AddOrReplaceStub(StubModel stubModel)
    {
        var item = StubCache.ContainsKey(stubModel.Id) ? StubCache[stubModel.Id] : null;
        if (item != null)
        {
            StubCache.Remove(stubModel.Id, out _);
        }

        if (!StubCache.TryAdd(stubModel.Id, stubModel))
        {
            _logger.LogWarning("Could not add stub with ID '{}' to cache.", stubModel.Id);
        }

        var metadata = UpdateMetadata(GetMetadataPath());
        UpdateLocalStubUpdateTrackingId(metadata.StubUpdateTrackingId);
    }

    /// <inheritdoc />
    public void DeleteStub(string stubId)
    {
        if (StubCache.TryRemove(stubId, out _))
        {
            var metadata = UpdateMetadata(GetMetadataPath());
            UpdateLocalStubUpdateTrackingId(metadata.StubUpdateTrackingId);
        }
    }

    internal FileStorageMetadataModel EnsureAndGetMetadata()
    {
        var path = GetMetadataPath();
        FileStorageMetadataModel model;
        if (!_fileService.FileExists(path))
        {
            model = UpdateMetadata(path);
        }
        else
        {
            var contents = _fileService.ReadAllText(path);
            model = JsonConvert.DeserializeObject<FileStorageMetadataModel>(contents);
        }

        return model;
    }

    private string GetMetadataPath()
    {
        var rootPath = _settings.Storage?.FileStorageLocation;
        var path = Path.Combine(rootPath, Constants.MetadataFileName);
        return path;
    }

    private FileStorageMetadataModel UpdateMetadata(string path)
    {
        FileStorageMetadataModel model;
        lock (_cacheUpdateLock)
        {
            model = new FileStorageMetadataModel {StubUpdateTrackingId = Guid.NewGuid().ToString()};
            _fileService.WriteAllText(path, JsonConvert.SerializeObject(model));
        }

        return model;
    }

    private void UpdateLocalStubUpdateTrackingId(string trackingId)
    {
        lock (_cacheUpdateLock)
        {
            StubUpdateTrackingId = trackingId;
        }
    }

    private string GetStubsFolder() =>
        Path.Combine(_settings.Storage?.FileStorageLocation, Constants.StubsFolderName);
}
