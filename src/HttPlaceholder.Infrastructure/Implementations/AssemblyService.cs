using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <inheritdoc />
public class AssemblyService : IAssemblyService
{
    /// <inheritdoc />
    public string GetAssemblyVersion() => AssemblyHelper.GetAssemblyVersion();

    /// <inheritdoc />
    public string GetCallingAssemblyRootPath() => AssemblyHelper.GetCallingAssemblyRootPath();

    /// <inheritdoc />
    public string GetEntryAssemblyRootPath() => AssemblyHelper.GetEntryAssemblyRootPath();

    /// <inheritdoc />
    public string GetExecutingAssemblyRootPath() => AssemblyHelper.GetExecutingAssemblyRootPath();
}
