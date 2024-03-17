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

internal class StubRootPathResolver(
    IAssemblyService assemblyService,
    IFileService fileService,
    IOptionsMonitor<SettingsModel> options)
    : IStubRootPathResolver, ISingletonService
{
    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetStubRootPathsAsync(CancellationToken cancellationToken)
    {
        var settings = options.CurrentValue;

        // First, check the "inputFile" configuration property and extract the directory of this folder.
        var inputFile = settings.Storage?.InputFile ?? "";
        var fileStorageLocation = settings.Storage?.FileStorageLocation ?? "";
        if (string.IsNullOrWhiteSpace(inputFile) && string.IsNullOrWhiteSpace(fileStorageLocation))
        {
            // If no input file was provided, return the assembly path instead.
            return new[] { assemblyService.GetEntryAssemblyRootPath() };
        }

        IEnumerable<string> result = !string.IsNullOrWhiteSpace(inputFile)
            ? await Task.WhenAll(
                inputFile.Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => GetDirectoryAsync(f, cancellationToken)))
            : Array.Empty<string>();
        if (!string.IsNullOrWhiteSpace(fileStorageLocation))
        {
            result = result.Concat(new[] { fileStorageLocation });
        }

        return result.Distinct();
    }

    private async Task<string> GetDirectoryAsync(string filename, CancellationToken cancellationToken) =>
        await fileService.IsDirectoryAsync(filename, cancellationToken) ? filename : Path.GetDirectoryName(filename);
}
