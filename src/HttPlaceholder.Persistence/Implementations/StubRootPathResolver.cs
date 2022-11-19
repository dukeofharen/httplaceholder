using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations;

internal class StubRootPathResolver : IStubRootPathResolver, ISingletonService
{
    private readonly IAssemblyService _assemblyService;
    private readonly IFileService _fileService;
    private readonly IOptionsMonitor<SettingsModel> _options;

    public StubRootPathResolver(
        IAssemblyService assemblyService,
        IFileService fileService,
        IOptionsMonitor<SettingsModel> options)
    {
        _assemblyService = assemblyService;
        _fileService = fileService;
        _options = options;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetStubRootPathsAsync(CancellationToken cancellationToken)
    {
        var settings = _options.CurrentValue;

        // First, check the "inputFile" configuration property and extract the directory of this folder.
        var inputFile = settings.Storage?.InputFile;
        if (inputFile == null)
        {
            // If no input file was provided, return the assembly path instead.
            return new[] {_assemblyService.GetEntryAssemblyRootPath()};
        }

        var result = await Task.WhenAll(
            inputFile.Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => GetDirectoryAsync(f, cancellationToken)));
        if (!string.IsNullOrWhiteSpace(settings.Storage?.FileStorageLocation))
        {
            result = result.Concat(new[] {settings.Storage.FileStorageLocation}).ToArray();
        }

        return result;
    }

    private async Task<string> GetDirectoryAsync(string filename, CancellationToken cancellationToken) =>
        await _fileService.IsDirectoryAsync(filename, cancellationToken) ? filename : Path.GetDirectoryName(filename);
}
