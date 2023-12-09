using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Common.FileWatchers;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpYaml;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to read data from one or several YAML files, from possibly multiple locations.
///     This source uses a file system watcher.
/// </summary>
internal class FileWatcherYamlFileStubSource(
    IFileService fileService,
    ILogger<FileWatcherYamlFileStubSource> logger,
    IOptionsMonitor<SettingsModel> options,
    IStubModelValidator stubModelValidator,
    IFileWatcherBuilderFactory fileWatcherBuilderFactory)
    : BaseFileStubSource(logger, fileService, options, stubModelValidator), IDisposable
{
    private readonly IFileWatcherBuilderFactory _fileWatcherBuilderFactory = fileWatcherBuilderFactory;
    internal readonly ConcurrentDictionary<string, FileSystemWatcher> FileSystemWatchers = new();

    // A dictionary that contains all the loaded stubs, grouped by file the stub is in.
    internal readonly ConcurrentDictionary<string, IEnumerable<StubModel>> Stubs = new();

    /// <inheritdoc />
    public override Task<IEnumerable<StubModel>> GetStubsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Stubs.Values.SelectMany(v => v));

    /// <inheritdoc />
    public override async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => new StubOverviewModel { Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled })
        .ToArray();

    /// <inheritdoc />
    public override Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        SetupStubs();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var watcher in FileSystemWatchers.Values)
        {
            watcher.Dispose();
        }
    }

    /// <inheritdoc />
    public override async Task<StubModel> GetStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken)).FirstOrDefault(s => s.Id == stubId);

    private void SetupStubs()
    {
        var locations = GetInputLocations();
        foreach (var location in locations)
        {
            if (!FileService.DirectoryExists(location) && !FileService.FileExists(location))
            {
                Logger.LogWarning($"Location '{location}' not found.");
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
        Logger.LogDebug($"Parsing file '{file}'.");
        try
        {
            Logger.LogDebug($"Trying to add and parse stubs for '{file}'.");
            var stubs = ParseAndValidateStubs(input, file);
            Stubs.AddOrUpdate(file, (k) => stubs, (k, v) => stubs);
        }
        catch (YamlException ex)
        {
            Logger.LogWarning(ex, $"Error occurred while parsing file '{file}'");
        }
    }

    private void SetupWatcherForLocation(string location)
    {
        var builder = _fileWatcherBuilderFactory.CreateBuilder();
        builder.SetPathOrFilters(location, SupportedExtensions);
        builder.SetNotifyFilters(NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size
                                 | NotifyFilters.Attributes
                                 | NotifyFilters.Security);
        builder.SetOnChanged(OnInputLocationUpdated);
        builder.SetOnCreated(OnInputLocationUpdated);
        builder.SetOnDeleted(OnInputLocationUpdated);
        builder.SetOnRenamed(OnInputLocationUpdated);
        builder.SetOnError(OnError);
        FileSystemWatchers.TryAdd(location, builder.Build());
    }

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
        Logger.LogDebug($"File {e.FullPath} changed.");
        LoadStubs(e.FullPath);
    }

    private void FileCreated(FileSystemEventArgs e)
    {
        var fullPath = e.FullPath;
        if (FileService.IsDirectory(fullPath))
        {
            // Ignore the event if the created object is a directory.
            return;
        }

        Logger.LogDebug($"File {fullPath} created.");
        LoadStubs(fullPath);
    }

    private void FileDeleted(FileSystemEventArgs e)
    {
        var fullPath = e.FullPath;

        // Due to limitations in .NET / FileSystemWatcher, no event is triggered when a directory is deleted.
        if (!FileService.IsDirectory(fullPath))
        {
            Logger.LogDebug($"File {fullPath} deleted.");
            if (Stubs.TryRemove(fullPath, out _))
            {
                Logger.LogDebug($"Removed stub '{fullPath}'.");
            }
        }

        TryRemoveWatcher(fullPath);
    }

    private void FileRenamed(RenamedEventArgs e)
    {
        var fullPath = e.FullPath;
        var oldPath = e.OldFullPath;

        // Due to limitations in .NET / FileSystemWatcher, no event is triggered when a directory is renamed.
        if (!FileService.IsDirectory(fullPath))
        {
            Logger.LogDebug($"File {fullPath} renamed.");

            // Try to delete the stub from memory.
            if (Stubs.TryRemove(oldPath, out _))
            {
                Logger.LogDebug($"Removed stub '{oldPath}'.");
            }

            TryRemoveWatcher(oldPath);
            SetupWatcherForLocation(fullPath);
            if (!SupportedExtensions.Any(ext => fullPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                Logger.LogDebug(
                    $"File {fullPath} not supported. Supported file extensions: {string.Join(", ", SupportedExtensions)}");
            }
            else
            {
                LoadStubs(fullPath);
            }
        }
    }

    private bool TryRemoveWatcher(string fullPath)
    {
        if (FileSystemWatchers.TryGetValue(fullPath, out var foundWatcher))
        {
            foundWatcher.Dispose();
            FileSystemWatchers.TryRemove(fullPath, out _);
            return true;
        }

        return false;
    }
}
