using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class AssemblyService : IAssemblyService, ISingletonService
{
    /// <inheritdoc />
    public string GetAssemblyVersion() => AssemblyHelper.GetAssemblyVersion();

    /// <inheritdoc />
    public string GetEntryAssemblyRootPath() => AssemblyHelper.GetEntryAssemblyRootPath();

    /// <inheritdoc />
    public string GetExecutingAssemblyRootPath() => AssemblyHelper.GetExecutingAssemblyRootPath();
}
