using HttPlaceholder.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HttPlaceholder.Models
{
   public class ConditionCheckResultModel
   {
      public string CheckerName { get; set; }

      [JsonConverter(typeof(StringEnumConverter))]
      public ConditionValidationType ConditionValidation { get; set; } = ConditionValidationType.NotExecuted;

      public string Log { get; set; }
   }
}
