using System;
using HttPlaceholder.Models.Enums;
using Newtonsoft.Json;

namespace HttPlaceholder.Models
{
    public class ConditionCheckResultModel
    {
        public string CheckerName { get; set; }

        [JsonIgnore]
        public ConditionValidationType ConditionValidation { get; set; } = ConditionValidationType.NotExecuted;

        [JsonProperty("ConditionValidation")]
        public string ConditionValidationText
        {
            get => ConditionValidation.ToString();
            set => ConditionValidation = (ConditionValidationType)Enum.Parse(typeof(ConditionValidationType), value);
        }

        public string Log { get; set; }

        public override string ToString()
        {
            return $"{CheckerName}: {ConditionValidation}";
        }
    }
}