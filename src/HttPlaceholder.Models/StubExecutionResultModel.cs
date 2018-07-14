using System.Collections.Generic;

namespace HttPlaceholder.Models
{
   public class StubExecutionResultModel
   {
      public string StubId { get; set; }

      public bool Passed { get; set; }

      public IEnumerable<ConditionCheckResultModel> Conditions { get; set; }

      public IEnumerable<ConditionCheckResultModel> NegativeConditions { get; set; }
   }
}
