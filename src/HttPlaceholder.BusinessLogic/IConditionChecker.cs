using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

namespace HttPlaceholder.BusinessLogic
{
   public interface IConditionChecker
   {
     ConditionValidationType Validate(string stubId, StubConditionsModel conditions);
   }
}
