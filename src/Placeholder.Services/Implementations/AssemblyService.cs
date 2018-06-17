using System.IO;
using System.Reflection;

namespace Placeholder.Services.Implementations
{
   internal class AssemblyService : IAssemblyService
   {
      public string GetAssemblyRootPath()
      {
         var assembly = Assembly.GetEntryAssembly();
         string path = assembly.Location;
         return Path.GetDirectoryName(path);
      }
   }
}
