using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.FileSystem.Implementations;

/// <inheritdoc />
internal class FileSystemStubCache(
    ILogger<FileSystemStubCache> logger,
    IFileService fileService,
    IOptionsMonitor<SettingsModel> options)
    : IFileSystemStubCache
{
    private static readonly object _cacheUpdateLock = new();

    internal readonly ConcurrentDictionary<string, StubModel> StubCache = new();
    internal string StubUpdateTrackingId;

    /// <inheritdoc />
    public async Task<IEnumerable<StubModel>> GetOrUpdateStubCacheAsync(string distributionKey,
        CancellationToken cancellationToken)
    {
        var path = GetStubsFolder(distributionKey);
        if (!string.IsNullOrWhiteSpace(distributionKey))
        {
            return await GetStubsAsync(path, cancellationToken);
        }

        var shouldUpdateCache = false;

        // Check if the "stub update tracking ID" variable has a new value.
        var metadata = EnsureAndGetMetadata();
        if (StubUpdateTrackingId == null)
        {
            // The local cache hasn't been initialized yet. Do that now.
            logger.LogDebug(
                "Initializing the cache, because either the local stub cache or tracking ID is not set yet.");
            UpdateLocalStubUpdateTrackingId(metadata?.StubUpdateTrackingId ?? Guid.NewGuid().ToString());
            shouldUpdateCache = true;
        }
        else if (StubUpdateTrackingId != metadata.StubUpdateTrackingId)
        {
            // ID has been changed. Update the stub cache.
            logger.LogDebug("Initializing the cache, because the tracking ID on disk has been changed.");
            UpdateLocalStubUpdateTrackingId(metadata.StubUpdateTrackingId);
            shouldUpdateCache = true;
        }

        if (!shouldUpdateCache)
        {
            return StubCache.Values;
        }

        StubCache.Clear();
        var newCache = await GetStubsAsync(path, cancellationToken);
        foreach (var item in newCache)
        {
            if (!StubCache.TryAdd(item.Id, item))
            {
                logger.LogWarning($"Could not add stub with ID '{item.Id}' to cache.");
            }
        }

        return StubCache.Values;
    }

    /// <inheritdoc />
    public void AddOrReplaceStub(StubModel stubModel)
    {
        var item = StubCache.TryGetValue(stubModel.Id, out var value) ? value : null;
        if (item != null)
        {
            StubCache.Remove(stubModel.Id, out _);
        }

        if (!StubCache.TryAdd(stubModel.Id, stubModel))
        {
            logger.LogWarning($"Could not add stub with ID '{stubModel.Id}' to cache.");
        }

        var metadata = UpdateMetadata(GetMetadataPath());
        UpdateLocalStubUpdateTrackingId(metadata.StubUpdateTrackingId);
    }

    /// <inheritdoc />
    public void DeleteStub(string stubId)
    {
        if (!StubCache.TryRemove(stubId, out _))
        {
            return;
        }

        var metadata = UpdateMetadata(GetMetadataPath());
        UpdateLocalStubUpdateTrackingId(metadata.StubUpdateTrackingId);
    }

    private async Task<IEnumerable<StubModel>> GetStubsAsync(string path, CancellationToken cancellationToken)
    {
        var files = await fileService.GetFilesAsync(path, "*.json", cancellationToken);
        return (await Task.WhenAll(files
                .Select(filePath => fileService
                    .ReadAllTextAsync(filePath, cancellationToken))))
            .Select(JsonConvert.DeserializeObject<StubModel>);
    }

    internal FileStorageMetadataModel EnsureAndGetMetadata()
    {
        var path = GetMetadataPath();
        FileStorageMetadataModel model;
        if (!fileService.FileExists(path))
        {
            model = UpdateMetadata(path);
        }
        else
        {
            var contents = fileService.ReadAllText(path);
            model = JsonConvert.DeserializeObject<FileStorageMetadataModel>(contents);
        }

        return model;
    }

    private string GetMetadataPath()
    {
        var rootPath = options.CurrentValue.Storage?.FileStorageLocation ??
                       throw new InvalidOperationException("FileStorageLocation unexpectedly null.");
        var path = Path.Combine(rootPath, FileNames.MetadataFileName);
        return path;
    }

    private FileStorageMetadataModel UpdateMetadata(string path)
    {
        FileStorageMetadataModel model;
        lock (_cacheUpdateLock)
        {
            model = new FileStorageMetadataModel { StubUpdateTrackingId = Guid.NewGuid().ToString() };
            fileService.WriteAllText(path, JsonConvert.SerializeObject(model));
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

    private string GetStubsFolder(string distributionKey)
    {
        var rootFolder = options?.CurrentValue?.Storage?.FileStorageLocation;
        return string.IsNullOrWhiteSpace(distributionKey)
            ? Path.Combine(rootFolder, FileNames.StubsFolderName)
            : Path.Combine(rootFolder, distributionKey, FileNames.StubsFolderName);
    }
}
