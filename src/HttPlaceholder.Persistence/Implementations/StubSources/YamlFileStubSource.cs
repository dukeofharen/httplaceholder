using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Core;
using Constants = HttPlaceholder.Domain.Constants;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
/// A stub source that is used to read data from one or several YAML files, from possibly multiple locations.
/// </summary>
internal class YamlFileStubSource : IStubSource
{
    private static readonly string[] _extensions = {".yml", ".yaml"};

    private IEnumerable<StubModel> _stubs;
    private DateTime _stubLoadDateTime;
    private readonly ILogger<YamlFileStubSource> _logger;
    private readonly IFileService _fileService;
    private readonly SettingsModel _settings;
    private readonly IStubModelValidator _stubModelValidator;

    public YamlFileStubSource(
        IFileService fileService,
        ILogger<YamlFileStubSource> logger,
        IOptions<SettingsModel> options,
        IStubModelValidator stubModelValidator)
    {
        _fileService = fileService;
        _logger = logger;
        _stubModelValidator = stubModelValidator;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubModel>> GetStubsAsync(CancellationToken cancellationToken)
    {
        var fileLocations = await GetYamlFileLocationsAsync(cancellationToken);
        if (fileLocations.Count == 0)
        {
            _logger.LogInformation("No .yml input files found.");
            return Array.Empty<StubModel>().AsEnumerable();
        }

        if (_stubs == null || GetLastStubFileModificationDateTime(fileLocations) > _stubLoadDateTime)
        {
            var result = new List<StubModel>();
            foreach (var file in fileLocations)
            {
                // Load the stubs.
                var input = await _fileService.ReadAllTextAsync(file, cancellationToken);
                _logger.LogInformation($"Parsing .yml file '{file}'.");
                try
                {
                    result.AddRange(ParseAndValidateStubs(input, file));
                    _stubLoadDateTime = DateTime.Now;
                }
                catch (YamlException ex)
                {
                    _logger.LogWarning(ex, $"Error occurred while parsing YAML file '{file}'");
                }
            }

            _stubs = result;
        }
        else
        {
            _logger.LogInformation("No stub file contents changed in the meanwhile.");
        }

        return _stubs;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken))
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
        .ToArray();

    /// <inheritdoc />
    public async Task<StubModel> GetStubAsync(string stubId, CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken)).FirstOrDefault(s => s.Id == stubId);

    /// <inheritdoc />
    public async Task PrepareStubSourceAsync(CancellationToken cancellationToken) =>
        // Check if the .yml files could be loaded.
        await GetStubsAsync(cancellationToken);

    private DateTime GetLastStubFileModificationDateTime(IEnumerable<string> files) =>
        files.Max(f => _fileService.GetLastWriteTime(f));

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
            if (validationResults.Any())
            {
                validationResults = validationResults.Select(r => $"- {r}").ToArray();
                _logger.LogWarning(
                    $"Validation warnings encountered for stub '{stub.Id}':\n{string.Join("\n", validationResults)}");
            }

            result.Add(stub);
        }

        return result;
    }

    private static string StripIllegalCharacters(string input) => input.Replace("\"", string.Empty);

    private static bool YamlIsArray(string yaml) => yaml
        .Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None)
        .Any(l => l.StartsWith("-"));

    private async Task<List<string>> GetYamlFileLocationsAsync(CancellationToken cancellationToken)
    {
        var inputFileLocation = _settings.Storage?.InputFile;
        var fileLocations = new List<string>();
        if (string.IsNullOrEmpty(inputFileLocation))
        {
            // If the input file location is not set, try looking in the current directory for .yml files.
            var currentDirectory = _fileService.GetCurrentDirectory();
            var yamlFiles = await _fileService.GetFilesAsync(currentDirectory, _extensions, cancellationToken);
            fileLocations.AddRange(yamlFiles);
        }
        else
        {
            // Split file path: it is possible to supply multiple locations.
            var parts = inputFileLocation.Split(Constants.InputFileSeparators,
                StringSplitOptions.RemoveEmptyEntries);
            parts = parts.Select(StripIllegalCharacters).ToArray();
            foreach (var part in parts)
            {
                var location = part.Trim();
                _logger.LogInformation($"Reading location '{location}'.");
                if (await _fileService.IsDirectoryAsync(location, cancellationToken))
                {
                    var yamlFiles = await _fileService.GetFilesAsync(location, _extensions, cancellationToken);
                    fileLocations.AddRange(yamlFiles);
                }
                else
                {
                    fileLocations.Add(location);
                }
            }
        }

        return fileLocations;
    }
}
