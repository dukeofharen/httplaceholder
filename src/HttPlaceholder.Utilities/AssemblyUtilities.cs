using System;
using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Utilities
{
    public static class AssemblyUtilities
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
    }
}