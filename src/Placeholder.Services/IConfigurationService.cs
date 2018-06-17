using System.Collections.Generic;

namespace Placeholder.Services
{
   public interface IConfigurationService
   {
      IDictionary<string, string> GetConfiguration();
   }
}
