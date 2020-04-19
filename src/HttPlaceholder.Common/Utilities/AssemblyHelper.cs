using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HttPlaceholder.Common.Utilities
{
    public static class AssemblyHelper
    {
        public static IEnumerable<Type> GetImplementations<TInterface>(string assemblyFilter = "")
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies();
            if (!string.IsNullOrWhiteSpace(assemblyFilter))
            {
                assemblies = assemblies.Where(a => a.FullName.Contains(assemblyFilter)).ToArray();
            }

            var types = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(TInterface).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToArray();

            return types;
        }

        public static string GetEntryAssemblyRootPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            var path = assembly.Location;
            return Path.GetDirectoryName(path);
        }

        public static string GetExecutingAssemblyRootPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var path = assembly.Location;
            return Path.GetDirectoryName(path);
        }

        public static string GetCallingAssemblyRootPath()
        {
            var assembly = Assembly.GetCallingAssembly();
            var path = assembly.Location;
            return Path.GetDirectoryName(path);
        }

        public static string GetAssemblyVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString();
        }
    }
}
