using System;
using System.Collections.Generic;

namespace HttPlaceholder.Models
{
   public class RequestResultModel
   {
      public string CorrelationId { get; set; }

      public object RequestParameters { get; set; }

      public IList<string> LogLines { get; set; } = new List<string>();

      public IList<object> StubExecutionResult { get; set; } = new List<object>();

      public string ExecutingStubId { get; set; }

      public DateTime RequestBeginTime { get; set; }

      public DateTime RequestEndTime { get; set; }
   }
}
