using System;
using System.Collections.Generic;
using System.Linq;
using Placeholder.Models;

namespace Placeholder.Services.Implementations
{
   internal class RequestLogger : IRequestLogger
   {
      private readonly RequestResultModel _result;

      public RequestLogger()
      {
         _result = new RequestResultModel
         {
            RequestBeginTime = DateTime.Now
         };
      }

      public void Log(string message)
      {
         _result.LogLines.Add(message);
      }

      public RequestResultModel GetResult()
      {
         _result.RequestEndTime = DateTime.Now;
         return _result;
      }

      public void LogRequestParameters(string method, string url, string body, IDictionary<string, string> headers)
      {
         string headerString = string.Join(", ", headers.Select(h => $"{h.Key} = {h.Value}"));
         _result.RequestParameters = new
         {
            method,
            url,
            body,
            headers = headerString
         };
      }

      public void SetCorrelationId(string correlationId)
      {
         _result.CorrelationId = correlationId;
      }

      public void SetStubExecutionResult(string stubId, bool passed)
      {
         _result.StubExecutionResult.Add(new
         {
            stubId,
            passed
         });
      }

      public void SetExecutingStubId(string stubId)
      {
         _result.ExecutingStubId = stubId;
      }
   }
}
