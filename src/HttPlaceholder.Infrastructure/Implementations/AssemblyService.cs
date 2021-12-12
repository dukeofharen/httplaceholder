using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Infrastructure.Implementations;

public class AssemblyService : IAssemblyService
{
    public string GetAssemblyVersion() => AssemblyHelper.GetAssemblyVersion();

    public string GetCallingAssemblyRootPath() => AssemblyHelper.GetCallingAssemblyRootPath();

    public string GetEntryAssemblyRootPath() => AssemblyHelper.GetEntryAssemblyRootPath();

    public string GetExecutingAssemblyRootPath() => AssemblyHelper.GetExecutingAssemblyRootPath();
}