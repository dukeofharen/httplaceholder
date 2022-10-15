using System;
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
    private readonly SettingsModel _settings;
    private readonly IFileService _fileService;

    public StubRootPathResolver(
        IAssemblyService assemblyService,
        IFileService fileService,
        IOptions<SettingsModel> options)
    {
        _assemblyService = assemblyService;
        _fileService = fileService;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public async Task<string[]> GetStubRootPathsAsync(CancellationToken cancellationToken)
    {
        // First, check the "inputFile" configuration property and extract the directory of this folder.
        var inputFile = _settings.Storage?.InputFile;
        if (inputFile == null)
        {
            // If no input file was provided, return the assembly path instead.
            return new[] {_assemblyService.GetEntryAssemblyRootPath()};
        }

        var tasks = inputFile.Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(f => GetDirectoryAsync(f, cancellationToken))
            .ToArray();
        return await Task.WhenAll(tasks);
    }

    private async Task<string> GetDirectoryAsync(string filename, CancellationToken cancellationToken) =>
        await _fileService.IsDirectoryAsync(filename, cancellationToken) ? filename : Path.GetDirectoryName(filename);
}
