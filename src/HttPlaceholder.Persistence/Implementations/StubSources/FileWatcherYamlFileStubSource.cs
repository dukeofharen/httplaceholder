using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpYaml;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to read data from one or several YAML files, from possibly multiple locations.
///     This source uses a file system watcher.
/// </summary>
internal class FileWatcherYamlFileStubSource : IStubSource, IDisposable
{
    private static readonly string[] _extensions = {".yml", ".yaml"};
    private readonly IFileService _fileService;
    private readonly ILogger<FileWatcherYamlFileStubSource> _logger;
    private readonly IOptionsMonitor<SettingsModel> _options;
    private readonly IStubModelValidator _stubModelValidator;
    private readonly ConcurrentDictionary<string, FileSystemWatcher> _fileSystemWatchers = new();

    // A dictionary that contains all the loaded stubs, grouped by file the stub is in.
    private readonly ConcurrentDictionary<string, IEnumerable<StubModel>> _stubs = new();

    public FileWatcherYamlFileStubSource(IFileService fileService, ILogger<FileWatcherYamlFileStubSource> logger,
        IOptionsMonitor<SettingsModel> options, IStubModelValidator stubModelValidator)
    {
        _fileService = fileService;
        _logger = logger;
        _options = options;
        _stubModelValidator = stubModelValidator;
    }

    /// <inheritdoc />
    public Task<IEnumerable<StubModel>> GetStubsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_stubs.Values.SelectMany(v => v));

    /// <inheritdoc />
    public async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
        .ToArray();

    /// <inheritdoc />
    public Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        SetupStubs();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var watcher in _fileSystemWatchers.Values)
        {
            watcher.Dispose();
        }
    }

    /// <inheritdoc />
    public async Task<StubModel> GetStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken)).FirstOrDefault(s => s.Id == stubId);

    private void SetupStubs()
    {
        var locations = GetInputLocations();
        foreach (var location in locations)
        {
            if (!_fileService.DirectoryExists(location) && !_fileService.FileExists(location))
            {
                _logger.LogWarning($"Location '{location}' not found.");
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
        var input = _fileService.ReadAllText(file);
        _logger.LogDebug($"Parsing file '{file}'.");
        try
        {
            _logger.LogDebug($"Trying to add and parse stubs for '{file}'.");
            var stubs = ParseAndValidateStubs(input, file);
            _stubs.AddOrUpdate(file, (k) => stubs, (k, v) => stubs);
        }
        catch (YamlException ex)
        {
            _logger.LogWarning(ex, $"Error occurred while parsing file '{file}'");
        }
    }

    private IEnumerable<StubModel> ParseAndValidateStubs(string input, string file)
    {
        IEnumerable<StubModel> stubs;
        if (YamlIsArray(input))
        {
            stubs = YamlUtilities.Parse<List<StubModel>>(input);
        }
        else
        {
            stubs = new[] {YamlUtilities.Parse<StubModel>(input)};
        }

        return ValidateStubs(file, stubs);
    }

    private IEnumerable<StubModel> ValidateStubs(string filename, IEnumerable<StubModel> stubs)
    {
        var result = new List<StubModel>();
        foreach (var stub in stubs)
        {
            if (string.IsNullOrWhiteSpace(stub?.Id))
            {
                // If no ID is set, log a warning as the stub is invalid.
                _logger.LogWarning($"Stub in file '{filename}' has no 'id' field defined, so is not a valid stub.");
                continue;
            }

            // Right now, stubs loaded from files are allowed to have validation errors.
            // They are NOT allowed to have no ID however.
            var validationResults = _stubModelValidator.ValidateStubModel(stub).ToArray();
            if (validationResults.Length != 0)
            {
                validationResults = validationResults.Select(r => $"- {r}").ToArray();
                _logger.LogWarning(
                    $"Validation warnings encountered for stub '{stub.Id}':\n{string.Join("\n", validationResults)}");
            }

            result.Add(stub);
        }

        return result;
    }

    private static bool YamlIsArray(string yaml) => yaml
        .SplitNewlines()
        .Any(l => l.StartsWith('-'));

    private void SetupWatcherForLocation(string location)
    {
        var isDir = _fileService.IsDirectory(location);
        var finalLocation = (isDir ? location : Path.GetDirectoryName(location)) ??
                            throw new InvalidOperationException($"Location {location} is invalid.");
        var watcher = new FileSystemWatcher(finalLocation);
        if (!isDir)
        {
            watcher.Filter = Path.GetFileName(location);
        }
        else
        {
            foreach (var extension in _extensions)
            {
                watcher.Filters.Add($"*{extension}");
            }
        }

        watcher.NotifyFilter = NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Size
                               | NotifyFilters.Attributes
                               | NotifyFilters.Security;
        watcher.Changed += OnInputLocationUpdated;
        watcher.Created += OnInputLocationUpdated;
        watcher.Deleted += OnInputLocationUpdated;
        watcher.Renamed += OnInputLocationUpdated;
        watcher.EnableRaisingEvents = true;
        _fileSystemWatchers.TryAdd(location, watcher);
    }

    private void OnInputLocationUpdated(object sender, FileSystemEventArgs e)
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
        _logger.LogDebug($"File {e.FullPath} changed.");
        LoadStubs(e.FullPath);
    }

    private void FileCreated(FileSystemEventArgs e)
    {
        var fullPath = e.FullPath;
        if (_fileService.IsDirectory(fullPath))
        {
            // Ignore the event if the created object is a directory.
            return;
        }

        _logger.LogDebug($"File {fullPath} created.");
        LoadStubs(fullPath);
    }

    private void FileDeleted(FileSystemEventArgs e)
    {
        var fullPath = e.FullPath;

        // Due to limitations in .NET / FileSystemWatcher, no event is triggered when a directory is deleted.
        if (!_fileService.IsDirectory(fullPath))
        {
            _logger.LogDebug($"File {fullPath} deleted.");
            if (_stubs.TryRemove(fullPath, out _))
            {
                _logger.LogDebug($"Removed stub '{fullPath}'.");
            }
        }

        TryRemoveWatcher(fullPath);
    }

    private void FileRenamed(RenamedEventArgs e)
    {
        var fullPath = e.FullPath;
        var oldPath = e.OldFullPath;
        // Due to limitations in .NET / FileSystemWatcher, no event is triggered when a directory is renamed.
        if (!_fileService.IsDirectory(fullPath))
        {
            _logger.LogDebug($"File {fullPath} renamed.");

            // Try to delete the stub from memory.
            if (_stubs.TryRemove(oldPath, out _))
            {
                _logger.LogDebug($"Removed stub '{oldPath}'.");
            }

            TryRemoveWatcher(oldPath);
            SetupWatcherForLocation(fullPath);
            if (!_extensions.Any(ext => fullPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogDebug(
                    $"File {fullPath} not supported. Supported file extensions: {string.Join(", ", _extensions)}");
            }
            else
            {
                LoadStubs(fullPath);
            }
        }
    }

    private IEnumerable<string> GetInputLocations()
    {
        var inputFileLocation = _options.CurrentValue.Storage?.InputFile;
        if (string.IsNullOrEmpty(inputFileLocation))
        {
            // If the input file location is not set, try looking in the current directory for files.
            var currentDirectory = _fileService.GetCurrentDirectory();
            return _fileService.GetFiles(currentDirectory, _extensions);
        }

        // Split file path: it is possible to supply multiple locations.
        return inputFileLocation
            .Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(StripIllegalCharacters);
    }

    private static string StripIllegalCharacters(string input) => input.Replace("\"", string.Empty);

    private IEnumerable<string> ParseFileLocations(string part)
    {
        var location = part.Trim();
        _logger.LogInformation($"Reading location '{location}'.");
        return _fileService.IsDirectory(location) ? _fileService.GetFiles(location, _extensions) : new[] {location};
    }

    private bool TryRemoveWatcher(string fullPath)
    {
        if (_fileSystemWatchers.TryGetValue(fullPath, out var foundWatcher))
        {
            foundWatcher.Dispose();
            _fileSystemWatchers.TryRemove(fullPath, out _);
            return true;
        }

        return false;
    }
}
