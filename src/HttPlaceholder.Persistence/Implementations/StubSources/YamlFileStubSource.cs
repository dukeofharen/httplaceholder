using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Core;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to read data from one or several YAML files, from possibly multiple locations.
/// </summary>
internal class YamlFileStubSource(
    IFileService fileService,
    ILogger<YamlFileStubSource> logger,
    IOptionsMonitor<SettingsModel> options,
    IStubModelValidator stubModelValidator)
    : BaseFileStubSource(logger, fileService, options, stubModelValidator)
{
    private DateTime _stubLoadDateTime;

    private IEnumerable<StubModel> _stubs;

    /// <inheritdoc />
    public override async Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var fileLocations = GetYamlFileLocations().ToArray();
        if (fileLocations.Length == 0)
        {
            Logger.LogInformation("No .yml input files found.");
            return Array.Empty<(StubModel, Dictionary<string, string>)>().AsEnumerable();
        }

        if (_stubs == null || GetLastStubFileModificationDateTime(fileLocations) > _stubLoadDateTime)
        {
            var result = new List<StubModel>();
            foreach (var location in fileLocations)
            {
                if (!await FileService.DirectoryExistsAsync(location, cancellationToken) &&
                    !await FileService.FileExistsAsync(location, cancellationToken))
                {
                    Logger.LogWarning($"Location '{location}' not found.");
                    continue;
                }

                result.AddRange(await LoadStubsAsync(location, cancellationToken));
            }

            _stubs = result;
        }
        else
        {
            Logger.LogDebug("No stub file contents changed in the meanwhile.");
        }

        return _stubs
            .Select(stub => (stub, new Dictionary<string, string>()));
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>>
        GetStubsOverviewAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => (new StubOverviewModel { Id = s.Stub.Id, Tenant = s.Stub.Tenant, Enabled = s.Stub.Enabled },
            s.Metadata));

    /// <inheritdoc />
    public override async Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(
        string stubId,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        {
            var result = (await GetStubsAsync(distributionKey, cancellationToken))
                .FirstOrDefault(s => s.Item1.Id == stubId);
            return result.Stub != null ? result : null;
        }
    }

    /// <inheritdoc />
    public override async Task PrepareStubSourceAsync(CancellationToken cancellationToken) =>
        // Check if the .yml files could be loaded.
        await GetStubsAsync(null, cancellationToken);

    private DateTime GetLastStubFileModificationDateTime(IEnumerable<string> files) =>
        files.Max(f => FileService.GetLastWriteTime(f));

    private async Task<IEnumerable<StubModel>> LoadStubsAsync(string file, CancellationToken cancellationToken)
    {
        // Load the stubs.
        var input = await FileService.ReadAllTextAsync(file, cancellationToken);
        Logger.LogInformation($"Parsing .yml file '{file}'.");
        try
        {
            var stubs = ParseAndValidateStubs(input, file);
            _stubLoadDateTime = DateTime.Now;
            return stubs;
        }
        catch (YamlException ex)
        {
            Logger.LogWarning(ex, $"Error occurred while parsing YAML file '{file}'");
        }

        return Array.Empty<StubModel>();
    }

    private IEnumerable<string> GetYamlFileLocations()
    {
        var locations = GetInputLocations();
        return locations.SelectMany(ParseFileLocations);
    }
}
