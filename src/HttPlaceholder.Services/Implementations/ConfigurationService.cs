using System;
using System.Collections.Generic;

namespace HttPlaceholder.Services.Implementations
{
   public class ConfigurationService : IConfigurationService
   {
      private static IDictionary<string, string> _configuration;

      public IDictionary<string, string> GetConfiguration()
      {
         return _configuration;
      }

      public static void StaticSetConfiguration(IDictionary<string, string> configuration)
      {
         if (_configuration != null)
         {
            throw new InvalidOperationException($"Can't set configuration again.");
         }

         _configuration = configuration;
      }
   }
}
