using System.Collections.Generic;
using System.Threading.Tasks;

namespace Placeholder.Implementation
{
   public interface IConditionChecker
   {
      Task<IEnumerable<string>> ValidateAsync(IEnumerable<string> stubIds);
   }
}
