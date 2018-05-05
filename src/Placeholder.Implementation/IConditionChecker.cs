using System.Collections.Generic;

namespace Placeholder.Implementation
{
   public interface IConditionChecker
   {
      IEnumerable<string> Validate(IEnumerable<string> stubIds);
   }
}
