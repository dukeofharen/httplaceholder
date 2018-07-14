using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic
{
   public interface IConditionChecker
   {
      ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions);
   }
}
