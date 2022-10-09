using System;
using System.Collections.Concurrent;
using System.IO;
using HttPlaceholder.Application.Interfaces.Resources;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Resources.Implementations;

/// <inheritdoc />
internal class ResourcesService : IResourcesService
{
    internal readonly ConcurrentDictionary<string, Lazy<string>> _resources = new();
    private readonly IFileService _fileService;

    /// <summary>
    /// Constructs a <see cref="ResourcesService"/> instance.
    /// </summary>
    public ResourcesService(IFileService fileService)
    {
        _fileService = fileService;
    }

    /// <inheritdoc />
    public string ReadAsString(string relativePath)
    {
        relativePath = PathUtilities.CleanPath(relativePath);
        var fullPath = Path.Join(AssemblyHelper.GetExecutingAssemblyRootPath(), relativePath);
        var lazy = _resources.GetOrAdd(relativePath, new Lazy<string>(() => _fileService.ReadAllText(fullPath)));
        return lazy.Value;
    }
}
