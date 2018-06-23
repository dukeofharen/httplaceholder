using System.Collections.Generic;
using Placeholder.Models;

namespace Placeholder.Services
{
   public interface IRequestLogger
   {
      void SetCorrelationId(string correlationId);

      void Log(string message);

      void LogRequestParameters(string method, string url, string body, IDictionary<string, string> headers);

      RequestResultModel GetResult();

      void SetStubExecutionResult(string stubId, bool passed);
   }
}
