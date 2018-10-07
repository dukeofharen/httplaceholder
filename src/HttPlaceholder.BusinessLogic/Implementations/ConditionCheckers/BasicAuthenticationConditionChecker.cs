using System;
using System.Text;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
    public class BasicAuthenticationConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public BasicAuthenticationConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var basicAuthenticationCondition = conditions?.BasicAuthentication;
            if (basicAuthenticationCondition != null)
            {
                var headers = _httpContextService.GetHeaders();

                // Try to retrieve the Authorization header.
                if (!headers.TryGetValue("Authorization", out string authorization))
                {
                    result.ConditionValidation = ConditionValidationType.Invalid;
                    result.Log = "No Authorization header found in request.";
                }
                else
                {
                    string expectedBase64UsernamePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicAuthenticationCondition.Username}:{basicAuthenticationCondition.Password}"));
                    string expectedAuthorizationHeader = $"Basic {expectedBase64UsernamePassword}";
                    if (expectedAuthorizationHeader == authorization)
                    {
                        result.Log = $"Basic authentication condition passed for stub '{stubId}'.";
                        result.ConditionValidation = ConditionValidationType.Valid;
                    }
                    else
                    {
                        result.Log = $"Basic authentication condition failed for stub '{stubId}'. Expected '{expectedAuthorizationHeader}' but found '{authorization}'.";
                        result.ConditionValidation = ConditionValidationType.Invalid;
                    }
                }
            }

            return result;
        }
    }
}