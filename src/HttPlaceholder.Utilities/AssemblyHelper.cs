using System;
using System.Collections.Generic;
using System.Linq;

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
   }
}
