using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Infrastructure.Implementations
{
    public class AssemblyService : IAssemblyService
    {
        public string GetAssemblyVersion()
        {
            return AssemblyHelper.GetAssemblyVersion();
        }

        public string GetCallingAssemblyRootPath()
        {
            return AssemblyHelper.GetCallingAssemblyRootPath();
        }

        public string GetEntryAssemblyRootPath()
        {
            return AssemblyHelper.GetEntryAssemblyRootPath();
        }

        public string GetExecutingAssemblyRootPath()
        {
            return AssemblyHelper.GetExecutingAssemblyRootPath();
        }
    }
}
