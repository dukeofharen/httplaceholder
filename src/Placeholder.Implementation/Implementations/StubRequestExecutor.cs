using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations
{
   internal class StubRequestExecutor : IStubRequestExecutor
   {
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubContainer _stubContainer;

      public StubRequestExecutor(
         IServiceProvider serviceProvider,
         IStubContainer stubContainer)
      {
         _serviceProvider = serviceProvider;
         _stubContainer = stubContainer;
      }

      public async Task<ResponseModel> ExecuteRequestAsync()
      {
         string finalStubId = null;
         var conditionCheckers = (IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker));
         foreach (var checker in conditionCheckers)
         {
            (ConditionCheckResultType result, string stubId) = await checker.ValidateAsync();
            if (result == ConditionCheckResultType.Invalid)
            {
               // TODO return response prematurely
            }
            else if (result == ConditionCheckResultType.Valid && string.IsNullOrEmpty(stubId))
            {
               // TODO throw exception here; this isn't right.
            }

            if (finalStubId != null && stubId != null && finalStubId != stubId)
            {
               // TODO throw exception here; this isn't right.
            }

            finalStubId = stubId;
         }

         // Retrieve stub and parse response.
         // TODO move this code to separate class.
         var stub = _stubContainer
            .Stubs
            .Select(s => s as IDictionary<object, object>)
            .Single(s => s?.Keys.Any(k => k.ToString() == "id") == true && s["id"].ToString() == finalStubId);
         if (stub != null)
         {
            var response = new ResponseModel
            {
               StatusCode = 200
            };

            var stubResponse = stub.FirstOrDefault(e => e.Key.ToString() == "response").Value as IDictionary<object, object>;

            var stubStatusCode = stubResponse.FirstOrDefault(s => s.Key.ToString() == "statusCode").Value;
            if (stubStatusCode != null)
            {
               response.StatusCode = int.Parse(stubStatusCode.ToString());
            }

            var stubResponseText = stubResponse.FirstOrDefault(s => s.Key.ToString() == "text").Value;
            if (stubResponseText != null)
            {
               response.Body = stubResponseText.ToString();
            }

            var stubResponseHeaders = stubResponse.FirstOrDefault(s => s.Key.ToString() == "headers").Value as IDictionary<object, object>;
            if (stubResponseHeaders != null)
            {
               foreach (var header in stubResponseHeaders)
               {
                  response.Headers.Add(header.Key.ToString(), header.Value.ToString());
               }
            }

            return response;
         }

         // TODO throw exception of zo
         throw new Exception();
      }
   }
}
