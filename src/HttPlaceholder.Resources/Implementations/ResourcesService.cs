using System;
using System.Collections.Concurrent;
using System.IO;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Resources;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Resources.Implementations;

internal class ResourcesService : IResourcesService, ISingletonService
{
    private readonly ConcurrentDictionary<string, Lazy<string>> _resources = new();
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
