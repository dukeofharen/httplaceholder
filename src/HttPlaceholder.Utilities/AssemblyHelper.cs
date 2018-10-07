using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HttPlaceholder.Utilities
{
    public static class AssemblyHelper
    {
        public static IEnumerable<Type> GetImplementations<TInterface>()
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => (typeof(TInterface).IsAssignableFrom(p)) && !p.IsInterface && !p.IsAbstract)
                .ToArray();

            return types;
        }

        public static string GetAssemblyRootPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            string path = assembly.Location;
            return Path.GetDirectoryName(path);
        }

        public static string GetExecutingAssemblyRootPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string path = assembly.Location;
            return Path.GetDirectoryName(path);
        }
    }
}