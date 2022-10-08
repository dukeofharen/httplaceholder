using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations;

/// <inheritdoc />
internal class StubRootPathResolver : IStubRootPathResolver
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
    public async Task<string[]> GetStubRootPathsAsync()
    {
        // First, check the "inputFile" configuration property and extract the directory of this folder.
        var inputFile = _settings.Storage?.InputFile;
        if (inputFile == null)
        {
            // If no input file was provided, return the assembly path instead.
            return new[] {_assemblyService.GetEntryAssemblyRootPath()};
        }

        var tasks = inputFile.Split(Constants.InputFileSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(GetDirectoryAsync)
            .ToArray();
        return await Task.WhenAll(tasks);
    }

    private async Task<string> GetDirectoryAsync(string filename) => await _fileService.IsDirectoryAsync(filename) ? filename : Path.GetDirectoryName(filename);
}
