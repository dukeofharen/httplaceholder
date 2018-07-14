using System;
using System.Collections.Generic;

namespace HttPlaceholder.Models
{
   public class RequestResultModel
   {
      public string CorrelationId { get; set; }

      public object RequestParameters { get; set; }

      public IList<StubExecutionResultModel> StubExecutionResults { get; set; } = new List<StubExecutionResultModel>();

      public string ExecutingStubId { get; set; }

      public DateTime RequestBeginTime { get; set; }

      public DateTime RequestEndTime { get; set; }
   }
}
