using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Exceptions;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations
{
   public class StubRequestExecutor : IStubRequestExecutor
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubManager _stubManager;

      public StubRequestExecutor(
         IRequestLoggerFactory requestLoggerFactory,
         IServiceProvider serviceProvider,
         IStubManager stubManager)
      {
         _requestLoggerFactory = requestLoggerFactory;
         _serviceProvider = serviceProvider;
         _stubManager = stubManager;
      }

      public ResponseModel ExecuteRequest()
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var conditionCheckers = ((IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker))).ToArray();
         requestLogger.Log($"Following conditions found: {string.Join(", ", conditionCheckers.Select(c => c.GetType().ToString()))}");

         var stubIds = new List<string>();
         var stubs = _stubManager.Stubs;
         foreach (var stub in stubs)
         {
            requestLogger.Log($"-------- CHECKING {stub.Id} --------");
            try
            {
               var validationResults = new List<ConditionValidationType>();
               foreach (var checker in conditionCheckers)
               {
                  requestLogger.Log($"----- EXECUTING CONDITION {checker.GetType().Name} -----");
                  var validationResult = checker.Validate(stub);
                  validationResults.Add(validationResult);
                  if (validationResult == ConditionValidationType.NotExecuted)
                  {
                     // If the resulting list is null, it means the check wasn't executed because it wasn't configured. Continue with the next condition.
                     requestLogger.Log("The condition was not executed and not configured.");
                     continue;
                  }

                  if (validationResult == ConditionValidationType.Invalid)
                  {
                     requestLogger.Log("The condition did not pass.");
                  }

                  requestLogger.Log($"----- DONE EXECUTING CONDITION {checker.GetType().Name} -----");
               }

               if (validationResults.All(r => r != ConditionValidationType.Invalid) && validationResults.Any(r => r != ConditionValidationType.NotExecuted && r != ConditionValidationType.NotSet))
               {
                  requestLogger.Log($"All conditions passed for stub '{stub.Id}'.");
                  stubIds.Add(stub.Id);
               }
            }
            catch (Exception e)
            {
               requestLogger.Log($"Exception thrown while executing condition checks for stub '{stub.Id}': {e}");
            }

            requestLogger.Log($"-------- DONE CHECKING {stub.Id} --------");
         }

         if (!stubIds.Any())
         {
            // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
            throw new RequestValidationException($"The '{nameof(stubIds)}' array for condition was empty, which means the condition was configured and the request did not pass or no conditions are configured at all.");
         }

         if (stubIds.Count > 1)
         {
            requestLogger.Log($"Multiple stubs are found ({string.Join(", ", stubIds)}), picking the first one.");
         }

         string finalStubId = stubIds.First();

         // Retrieve stub and parse response.
         var finalStub = _stubManager.GetStubById(finalStubId);
         if (finalStub != null)
         {
            requestLogger.Log($"Stub with ID '{finalStubId}' found; returning response.");
            var response = new ResponseModel
            {
               StatusCode = 200
            };

            response.StatusCode = finalStub.Response?.StatusCode ?? 200;
            requestLogger.Log($"Found HTTP status code '{response.StatusCode}'.");

            if (finalStub.Response?.Text != null)
            {
               response.Body = Encoding.UTF8.GetBytes(finalStub.Response.Text);
               requestLogger.Log($"Found body '{finalStub.Response?.Text}'");
            }
            else if (finalStub.Response?.Base64 != null)
            {
               string base64Body = finalStub.Response.Base64;
               response.Body = Convert.FromBase64String(base64Body);
               string bodyForLogging = base64Body.Length > 10 ? base64Body.Substring(0, 10) : base64Body;
               requestLogger.Log($"Found base64 body: {bodyForLogging}");
            }

            var stubResponseHeaders = finalStub.Response?.Headers;
            if (stubResponseHeaders != null)
            {
               foreach (var header in stubResponseHeaders)
               {
                  requestLogger.Log($"Found header '{header.Key}' with value '{header.Value}'.");
                  response.Headers.Add(header.Key, header.Value);
               }
            }

            return response;
         }

         throw new RequestValidationException($"Stub with ID '{finalStubId}' unexpectedly not found.");
      }
   }
}
