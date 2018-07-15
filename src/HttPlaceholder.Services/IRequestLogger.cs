using System.Collections.Generic;
using HttPlaceholder.Models;

namespace HttPlaceholder.Services
{
   public interface IRequestLogger
   {
      void SetCorrelationId(string correlationId);

      void LogRequestParameters(string method, string url, string body, string clientIp, IDictionary<string, string> headers);

      RequestResultModel GetResult();

      void SetStubExecutionResult(string stubId, bool passed, IEnumerable<ConditionCheckResultModel> conditions, IEnumerable<ConditionCheckResultModel> negativeConditions);

      void SetResponseWriterResult(string writerName, bool executed);

      void SetExecutingStubId(string stubId);
   }
}
