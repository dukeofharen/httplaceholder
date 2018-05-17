using System;
using System.Text;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class BasicAuthenticationConditionChecker : IConditionChecker
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IHttpContextService _httpContextService;

      public BasicAuthenticationConditionChecker(
         IRequestLoggerFactory requestLoggerFactory,
         IHttpContextService httpContextService)
      {
         _requestLoggerFactory = requestLoggerFactory;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(StubModel stub)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var result = ConditionValidationType.NotExecuted;
         var basicAuthenticationCondition = stub.Conditions?.BasicAuthentication;
         if (basicAuthenticationCondition != null)
         {
            requestLogger.Log($"Basic authentication condition found for stub '{stub.Id}': '{basicAuthenticationCondition}'");
            var headers = _httpContextService.GetHeaders();

            // Try to retrieve the Authorization header.
            if (!headers.TryGetValue("Authorization", out string authorization))
            {
               requestLogger.Log("No Authorization header found in request.");
               result = ConditionValidationType.Invalid;
            }
            else
            {
               string expectedBase64UsernamePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicAuthenticationCondition.Username}:{basicAuthenticationCondition.Password}"));
               string expectedAuthorizationHeader = $"Basic {expectedBase64UsernamePassword}";
               if (expectedAuthorizationHeader == authorization)
               {
                  requestLogger.Log($"Basic authentication condition passed for stub '{stub.Id}'.");
                  result = ConditionValidationType.Valid;
               }
               else
               {
                  requestLogger.Log($"Basic authentication condition failed for stub '{stub.Id}'. Expected '{expectedAuthorizationHeader}' but found '{authorization}'.");
                  result = ConditionValidationType.Invalid;
               }
            }
         }

         return result;
      }
   }
}
