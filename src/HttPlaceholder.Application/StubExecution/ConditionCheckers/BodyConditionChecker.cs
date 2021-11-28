using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers
{
    public class BodyConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public BodyConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var bodyConditions = stub.Conditions?.Body?.ToArray();
            if (bodyConditions == null || bodyConditions?.Any() != true)
            {
                return result;
            }

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
            result.ConditionValidation = validBodyConditions == bodyConditions.Length
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;

            return result;
        }

        public int Priority => 8;
    }
}
