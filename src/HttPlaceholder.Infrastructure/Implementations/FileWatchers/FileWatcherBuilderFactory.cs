using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.FileWatchers;

namespace HttPlaceholder.Infrastructure.Implementations.FileWatchers;

internal class FileWatcherBuilderFactory(IFileService fileService) : IFileWatcherBuilderFactory, ISingletonService
{
    public IFileWatcherBuilder CreateBuilder() => new FileWatcherBuilder(fileService);
}
