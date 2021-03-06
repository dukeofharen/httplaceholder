﻿using System;
using System.Text;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
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
            if (basicAuthenticationCondition == null ||
                (string.IsNullOrWhiteSpace(basicAuthenticationCondition.Username) &&
                 string.IsNullOrWhiteSpace(basicAuthenticationCondition.Password)))
            {
                return result;
            }

            var headers = _httpContextService.GetHeaders();

            // Try to retrieve the Authorization header.
            if (!headers.TryGetValue("Authorization", out var authorization))
            {
                result.ConditionValidation = ConditionValidationType.Invalid;
                result.Log = "No Authorization header found in request.";
            }
            else
            {
                var expectedBase64UsernamePassword = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        $"{basicAuthenticationCondition.Username}:{basicAuthenticationCondition.Password}"));
                var expectedAuthorizationHeader = $"Basic {expectedBase64UsernamePassword}";
                if (expectedAuthorizationHeader == authorization)
                {
                    result.Log = $"Basic authentication condition passed for stub '{stubId}'.";
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
                else
                {
                    result.Log =
                        $"Basic authentication condition failed for stub '{stubId}'. Expected '{expectedAuthorizationHeader}' but found '{authorization}'.";
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
            }

            return result;
        }

        public int Priority => 9;
    }
}
