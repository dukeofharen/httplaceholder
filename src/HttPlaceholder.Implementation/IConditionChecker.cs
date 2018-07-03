using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

namespace HttPlaceholder.Implementation
{
   public interface IConditionChecker
   {
     ConditionValidationType Validate(string stubId, StubConditionsModel conditions);
   }
}
