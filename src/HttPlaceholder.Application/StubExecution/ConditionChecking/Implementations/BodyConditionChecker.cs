using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class BodyConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public BodyConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var bodyConditions = conditions?.Body?.ToArray();
            if (bodyConditions != null && bodyConditions?.Any() == true)
            {
                var body = _httpContextService.GetBody();

                var validBodyConditions = 0;
                foreach (var condition in bodyConditions)
                {
                    if (!StringHelper.IsRegexMatchOrSubstring(body, condition))
                    {
                        // If the check failed, it means the query string is incorrect and the condition should fail.
                        result.Log = $"Body condition '{condition}' failed.";
                        break;
                    }

                    validBodyConditions++;
                }

                // If the number of succeeded conditions is equal to the actual number of conditions,
                // the body condition is passed and the stub ID is passed to the result.
                if (validBodyConditions == bodyConditions.Length)
                {
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
                else
                {
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
            }

            return result;
        }
    }
}
