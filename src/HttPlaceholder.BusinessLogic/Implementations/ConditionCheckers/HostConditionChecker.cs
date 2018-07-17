using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using HttPlaceholder.Utilities;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
   public class HostConditionChecker : IConditionChecker
   {
      private readonly IHttpContextService _httpContextService;

      public HostConditionChecker(IHttpContextService httpContextService)
      {
         _httpContextService = httpContextService;
      }

      public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
      {
         var result = new ConditionCheckResultModel();
         var hostCondition = conditions?.Host;
         if (hostCondition != null)
         {
            string host = _httpContextService.GetHost();
            if (!StringHelper.IsRegexMatchOrSubstring(host, hostCondition))
            {
               result.ConditionValidation = ConditionValidationType.Invalid;
            }
            else
            {
               result.ConditionValidation = ConditionValidationType.Valid;
            }
         }

         return result;
      }
   }
}
