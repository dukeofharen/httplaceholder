using HttPlaceholder.Utilities;

namespace HttPlaceholder.Services.Implementations
{
    internal class AssemblyService : IAssemblyService
    {
        public string GetAssemblyRootPath()
        {
            return AssemblyHelper.GetAssemblyRootPath();
        }

        public string GetAssemblyVersion()
        {
            return AssemblyHelper.GetAssemblyVersion();
        }
    }
}