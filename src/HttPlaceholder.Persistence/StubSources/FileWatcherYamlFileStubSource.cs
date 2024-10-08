﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Common.FileWatchers;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Core;

namespace HttPlaceholder.Persistence.StubSources;

/// <summary>
///     A stub source that is used to read data from one or several YAML files, from possibly multiple locations.
///     This source uses a file system watcher.
/// </summary>
internal class FileWatcherYamlFileStubSource(
    IFileService fileService,
    ILogger<FileWatcherYamlFileStubSource> logger,
    IOptionsMonitor<SettingsModel> options,
    IStubModelValidator stubModelValidator,
    IFileWatcherBuilderFactory fileWatcherBuilderFactory,
    IStubNotify stubNotify)
    : BaseFileStubSource(logger, fileService, options, stubModelValidator), IDisposable
{
    internal readonly ConcurrentDictionary<string, FileSystemWatcher> FileSystemWatchers = new();

    // A dictionary that contains all the loaded stubs, grouped by file the stub is in.
    internal readonly ConcurrentDictionary<string, IEnumerable<StubModel>> Stubs = new();

    public void Dispose()
    {
        foreach (var watcher in FileSystemWatchers.Values)
        {
            watcher.Dispose();
        }
    }

    /// <inheritdoc />
    public override Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (from kv
                in Stubs
            from stub
                in kv.Value
            select (stub,
                new Dictionary<string, string> { { StubMetadataKeys.Filename, kv.Key } })).AsTask();

    /// <inheritdoc />
    public override async Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>>
        GetStubsOverviewAsync(
            string distributionKey = null,
            CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => (new StubOverviewModel { Id = s.Stub.Id, Tenant = s.Stub.Tenant, Enabled = s.Stub.Enabled },
            s.Metadata))
        .ToArray();

    /// <inheritdoc />
    public override Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        SetupStubs();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(string stubId,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var result = (await GetStubsAsync(distributionKey, cancellationToken))
            .FirstOrDefault(s => s.Item1.Id == stubId);
        return result.Stub != null ? result : null;
    }

    private void SetupStubs()
    {
        var locations = GetInputLocations();
        foreach (var location in locations)
        {
            if (!FileService.DirectoryExists(location) && !FileService.FileExists(location))
            {
                Logger.LogWarning("Location '{Location}' not found.", location);
                continue;
            }

            SetupWatcherForLocation(location);

            var files = ParseFileLocations(location);
            foreach (var file in files)
            {
                LoadStubs(file);
            }
        }
    }

    private void LoadStubs(string file)
    {
        // Load the stubs.
        var input = FileService.ReadAllText(file);
        Logger.LogDebug("Parsing file '{File}'.", file);
        try
        {
            Logger.LogDebug("Trying to add and parse stubs for '{File}'.", file);
            var stubs = ParseAndValidateStubs(input, file);
            Stubs.AddOrUpdate(file, _ => stubs, (_, _) => stubs);
        }
        catch (YamlException ex)
        {
            Logger.LogWarning(ex, "Error occurred while parsing file '{File}'.", file);
        }
    }

    internal void SetupWatcherForLocation(string location) =>
        FileSystemWatchers.TryAdd(location, fileWatcherBuilderFactory.CreateBuilder()
            .SetPathOrFilters(location, SupportedExtensions)
            .SetNotifyFilters(NotifyFilters.CreationTime
                              | NotifyFilters.DirectoryName
                              | NotifyFilters.FileName
                              | NotifyFilters.LastWrite
                              | NotifyFilters.Size
                              | NotifyFilters.Attributes
                              | NotifyFilters.Security)
            .SetOnChanged(OnInputLocationUpdated)
            .SetOnCreated(OnInputLocationUpdated)
            .SetOnDeleted(OnInputLocationUpdated)
            .SetOnRenamed(OnInputLocationUpdated)
            .SetOnError(OnError)
            .SetEnableRaisingEvents(true)
            .Build());

    private void OnError(object sender, ErrorEventArgs e) =>
        Logger.LogWarning(e.GetException(), "Error occurred in file watcher.");

    internal void OnInputLocationUpdated(object sender, FileSystemEventArgs e)
    {
        switch (e.ChangeType)
        {
            case WatcherChangeTypes.Changed:
                FileChanged(e);
                break;
            case WatcherChangeTypes.Created:
                FileCreated(e);
                break;
            case WatcherChangeTypes.Deleted:
                FileDeleted(e);
                break;
            case WatcherChangeTypes.Renamed:
                FileRenamed((RenamedEventArgs)e);
                break;
        }
    }

    private void FileChanged(FileSystemEventArgs e)
    {
        Logger.LogDebug("File {File} changed.", e.FullPath);
        LoadStubs(e.FullPath);
        SignalStubsReload();
    }

    private void FileCreated(FileSystemEventArgs e)
    {
        var fullPath = e.FullPath;
        if (FileService.IsDirectory(fullPath))
        {
            // Ignore the event if the created object is a directory.
            return;
        }

        Logger.LogDebug("File {File} created.", fullPath);
        LoadStubs(fullPath);
        SignalStubsReload();
    }

    private void FileDeleted(FileSystemEventArgs e)
    {
        var fullPath = e.FullPath;

        // Due to limitations in .NET / FileSystemWatcher, no event is triggered when a directory is deleted.
        if (!FileService.IsDirectory(fullPath))
        {
            Logger.LogDebug("File {File} deleted.", fullPath);
            if (Stubs.TryRemove(fullPath, out _))
            {
                Logger.LogDebug("Removed stub '{File}'.", fullPath);
            }
        }

        TryRemoveWatcher(fullPath);
        SignalStubsReload();
    }

    private void FileRenamed(RenamedEventArgs e)
    {
        var fullPath = e.FullPath;
        var oldPath = e.OldFullPath;

        // Due to limitations in .NET / FileSystemWatcher, no event is triggered when a directory is renamed.
        if (!FileService.IsDirectory(fullPath))
        {
            Logger.LogDebug("File {OldPath} renamed to {NewPath}.", oldPath, fullPath);

            // Try to delete the stub from memory.
            if (Stubs.TryRemove(oldPath, out _))
            {
                Logger.LogDebug("Removed stub '{OldPath}'.", oldPath);
            }

            TryRemoveWatcher(oldPath);
            if (!SupportedExtensions.Any(ext => fullPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                Logger.LogDebug(
                    "File {NewPath} not supported. Supported file extensions: {Extensions}", fullPath,
                    string.Join(", ", SupportedExtensions));
            }
            else
            {
                LoadStubs(fullPath);
                SetupWatcherForLocation(fullPath);
            }
        }

        SignalStubsReload();
    }

    internal bool TryRemoveWatcher(string fullPath)
    {
        if (!FileSystemWatchers.TryGetValue(fullPath, out var foundWatcher))
        {
            return false;
        }

        foundWatcher.Dispose();
        FileSystemWatchers.TryRemove(fullPath, out _);
        return true;
    }

    private void SignalStubsReload() => stubNotify.ReloadStubsAsync().Wait();
}
