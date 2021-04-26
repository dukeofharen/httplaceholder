using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.FileSystem.Implementations
{
    internal class FileSystemStubCache : IFileSystemStubCache
    {
        private static readonly object _cacheUpdateLock = new object();
        internal string StubUpdateTrackingId;
        internal IList<StubModel> StubCache;

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

        public IEnumerable<StubModel> GetOrUpdateStubCache()
        {
            var shouldUpdateCache = false;

            // Check if the "stub update tracking ID" variable has a new value.
            var metadata = EnsureAndGetMetadata();
            if (StubCache == null || StubUpdateTrackingId == null)
            {
                // The local cache hasn't been initialized yet. Do that now.
                _logger.LogInformation(
                    "Initializing the cache, because either the local stub cache or tracking ID is not set yet.");
                StubUpdateTrackingId = metadata.StubUpdateTrackingId;
                shouldUpdateCache = true;
            }
            else if (StubUpdateTrackingId != metadata.StubUpdateTrackingId)
            {
                // ID has been changed. Update the stub cache.
                _logger.LogInformation("Initializing the cache, because the tracking ID on disk has been changed.");
                StubUpdateTrackingId = metadata.StubUpdateTrackingId;
                shouldUpdateCache = true;
            }

            if (shouldUpdateCache)
            {
                lock (_cacheUpdateLock)
                {
                    var path = GetStubsFolder();
                    var files = _fileService.GetFiles(path, "*.json");
                    StubCache = files
                        .Select(filePath => _fileService
                            .ReadAllText(filePath))
                        .Select(JsonConvert.DeserializeObject<StubModel>).ToList();
                }
            }

            return StubCache;
        }

        public void ClearStubCache()
        {
            // Clear the in memory stub cache.
            _logger.LogInformation("Clearing the file system stub cache.");
            var metadata = EnsureAndGetMetadata();
            lock (_cacheUpdateLock)
            {
                StubCache = null;
                var newId = Guid.NewGuid().ToString();
                StubUpdateTrackingId = newId;
                metadata.StubUpdateTrackingId = newId;
                _fileService.WriteAllText(
                    Path.Combine(_settings.Storage?.FileStorageLocation, Constants.MetadataFileName),
                    JsonConvert.SerializeObject(metadata));
            }
        }

        internal FileStorageMetadataModel EnsureAndGetMetadata()
        {
            var rootPath = _settings.Storage?.FileStorageLocation;
            var path = Path.Combine(rootPath, Constants.MetadataFileName);
            FileStorageMetadataModel model;
            if (!_fileService.FileExists(path))
            {
                lock (_cacheUpdateLock)
                {
                    model = new FileStorageMetadataModel {StubUpdateTrackingId = Guid.NewGuid().ToString()};
                    _fileService.WriteAllText(path, JsonConvert.SerializeObject(model));
                }
            }
            else
            {
                var contents = _fileService.ReadAllText(path);
                model = JsonConvert.DeserializeObject<FileStorageMetadataModel>(contents);
            }

            return model;
        }

        private string GetStubsFolder() =>
            Path.Combine(_settings.Storage?.FileStorageLocation, Constants.StubsFolderName);
    }
}
