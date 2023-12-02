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
    private readonly IList<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();

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
    public async Task<StubModel> GetStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken)).FirstOrDefault(s => s.Id == stubId);

    /// <inheritdoc />
    public async Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        // TODO
        // - File watchers inrichten.
        // - Alle .yaml files inladen samen met de file watchers (in SetupFileWatchers methode die dan wellicht anders moet heten).
        // - Alle stubs in memory inladen met bestandsnaam als key. Iedere keer als er een wijziging plaatsvindt; alléén dit specifiek bestand in geheugen aanpassen.
        // - De methodes implementeren.
        // - Unit tests maken.
        await SetupStubsAsync(cancellationToken);
        // await GetStubsAsync(null, cancellationToken);
    }

    private async Task SetupStubsAsync(CancellationToken cancellationToken)
    {
        var locations = await GetInputLocationsAsync(cancellationToken);
        foreach (var location in locations)
        {
            _fileSystemWatchers.Add(await SetupWatcherForLocation(location, cancellationToken));
            var files = await ParseFileLocationsAsync(location, cancellationToken);
            foreach (var file in files)
            {
                await LoadStubsAsync(file, cancellationToken);
            }
        }
    }

    private async Task LoadStubsAsync(string file, CancellationToken cancellationToken)
    {
        // Load the stubs.
        var input = await _fileService.ReadAllTextAsync(file, cancellationToken);
        _logger.LogInformation($"Parsing .yml file '{file}'.");
        try
        {
            _logger.LogDebug($"Trying to add and parse stubs for '{file}'.");
            var stubs = ParseAndValidateStubs(input, file);
            _stubs.AddOrUpdate(file, (k) => stubs, (k, v) => stubs);
        }
        catch (YamlException ex)
        {
            _logger.LogWarning(ex, $"Error occurred while parsing YAML file '{file}'");
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
            if (string.IsNullOrWhiteSpace(stub.Id))
            {
                // If no ID is set, log a warning as the stub is invalid.
                _logger.LogWarning($"Stub in file '{filename}' has no 'id' field defined, so is not a valid stub.");
                continue;
            }

            // Right now, stubs loaded from YAML files are allowed to have validation errors.
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

    private async Task<FileSystemWatcher> SetupWatcherForLocation(string location, CancellationToken cancellationToken)
    {
        var isDir = await _fileService.IsDirectoryAsync(location, cancellationToken);
        var finalLocation = (isDir ? location : Path.GetDirectoryName(location)) ??
                            throw new InvalidOperationException($"Location {location} is invalid.");
        var watcher = new FileSystemWatcher(finalLocation);
        if (isDir)
        {
            watcher.Filters.Add("*.yml");
            watcher.Filters.Add("*.yaml");
        }
        else
        {
            watcher.Filter = Path.GetFileName(location);
        }

        watcher.NotifyFilter = NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Size;
        watcher.Changed += OnInputLocationUpdated;
        watcher.Created += OnInputLocationUpdated;
        watcher.Deleted += OnInputLocationUpdated;
        watcher.Renamed += OnInputLocationUpdated;
        watcher.EnableRaisingEvents = true;
        return watcher;
    }

    private void OnInputLocationUpdated(object sender, FileSystemEventArgs e)
    {
        switch (e.ChangeType)
        {
            case WatcherChangeTypes.Changed:
                _logger.LogDebug($"File {e.FullPath} changed.");
                break;
            case WatcherChangeTypes.Created:
                _logger.LogDebug($"File {e.FullPath} created.");
                break;
            case WatcherChangeTypes.Deleted:
                _logger.LogDebug($"File {e.FullPath} deleted.");
                break;
            case WatcherChangeTypes.Renamed:
                _logger.LogDebug($"File {e.FullPath} renamed.");
                break;
        }
    }

    private async Task<IEnumerable<string>> GetInputLocationsAsync(CancellationToken cancellationToken)
    {
        var inputFileLocation = _options.CurrentValue.Storage?.InputFile;
        if (string.IsNullOrEmpty(inputFileLocation))
        {
            // If the input file location is not set, try looking in the current directory for .yml files.
            var currentDirectory = _fileService.GetCurrentDirectory();
            return await _fileService.GetFilesAsync(currentDirectory, _extensions, cancellationToken);
        }

        // Split file path: it is possible to supply multiple locations.
        return inputFileLocation
            .Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(StripIllegalCharacters);
    }

    private static string StripIllegalCharacters(string input) => input.Replace("\"", string.Empty);

    private async Task<IEnumerable<string>> ParseFileLocationsAsync(string part, CancellationToken cancellationToken)
    {
        var location = part.Trim();
        _logger.LogInformation($"Reading location '{location}'.");
        if (await _fileService.IsDirectoryAsync(location, cancellationToken))
        {
            return await _fileService.GetFilesAsync(location, _extensions, cancellationToken);
        }

        return new[] {location};
    }

    public void Dispose()
    {
        foreach (var watcher in _fileSystemWatchers)
        {
            watcher.Dispose();
        }
    }
}
