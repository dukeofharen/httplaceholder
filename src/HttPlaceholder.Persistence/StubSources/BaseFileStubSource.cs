﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.StubSources;

/// <summary>
///     A base class that is used for a read only stub source that reads files from the OS.
/// </summary>
internal abstract class BaseFileStubSource(
    ILogger<BaseFileStubSource> logger,
    IFileService fileService,
    IOptionsMonitor<SettingsModel> options,
    IStubModelValidator stubModelValidator)
    : IStubSource
{
    private static readonly string[] _extensions = [".yml", ".yaml"];
    protected readonly IFileService FileService = fileService;
    protected readonly ILogger<BaseFileStubSource> Logger = logger;

    protected static IEnumerable<string> SupportedExtensions => _extensions;

    public abstract Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>>
        GetStubsOverviewAsync(string distributionKey = null,
            CancellationToken cancellationToken = default);

    public abstract Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(string stubId,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    public abstract Task PrepareStubSourceAsync(CancellationToken cancellationToken);

    private static string StripIllegalCharacters(string input) => input.Replace("\"", string.Empty);

    protected IEnumerable<string> ParseFileLocations(string part)
    {
        var location = part.Trim();
        Logger.LogInformation("Reading location '{Location}'.", location);
        return FileService.IsDirectory(location) ? FileService.GetFiles(location, _extensions) : [location];
    }

    protected IEnumerable<string> GetInputLocations()
    {
        var inputFileLocation = options.CurrentValue.Storage?.InputFile;
        if (!string.IsNullOrEmpty(inputFileLocation))
        {
            // Split file path: it is possible to supply multiple locations.
            return inputFileLocation
                .Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(StripIllegalCharacters);
        }

        // If the input file location is not set, try looking in the current directory for files.
        var currentDirectory = FileService.GetCurrentDirectory();
        return FileService.GetFiles(currentDirectory, SupportedExtensions);
    }

    protected IEnumerable<StubModel> ParseAndValidateStubs(string input, string file)
    {
        var stubs = YamlIsArray(input)
            ? YamlUtilities.Parse<IEnumerable<StubModel>>(input)
            : new[] { YamlUtilities.Parse<StubModel>(input) };
        return ValidateStubs(file, stubs);
    }

    private static bool YamlIsArray(string yaml) => yaml
        .SplitNewlines()
        .Any(l => l.StartsWith('-'));

    private List<StubModel> ValidateStubs(string filename, IEnumerable<StubModel> stubs)
    {
        var result = new List<StubModel>();
        foreach (var stub in stubs)
        {
            if (string.IsNullOrWhiteSpace(stub?.Id))
            {
                // If no ID is set, log a warning as the stub is invalid.
                Logger.LogWarning("Stub in file '{Filename}' has no 'id' field defined, so is not a valid stub.",
                    filename);
                continue;
            }

            // Right now, stubs loaded from files are allowed to have validation errors.
            // They are NOT allowed to have no ID however.
            var validationResults = stubModelValidator.ValidateStubModel(stub)
                .Select(r => $"- {r}")
                .ToArray();
            if (validationResults.Length != 0)
            {
                Logger.LogWarning(
                    "Validation warnings encountered for stub '{StubId}':\n{ValidationErrors}", stub.Id,
                    string.Join("\n", validationResults));
            }

            result.Add(stub);
        }

        return result;
    }
}
