using System;
using HttPlaceholder.Models.Enums;
using Newtonsoft.Json;

namespace HttPlaceholder.Models
{
    /// <summary>
    /// A model for storing a condition check result.
    /// </summary>
    public class ConditionCheckResultModel
    {
        /// <summary>
        /// Gets or sets the name of the checker.
        /// </summary>
        public string CheckerName { get; set; }

        /// <summary>
        /// Gets or sets the condition validation.
        /// </summary>
        [JsonIgnore]
        public ConditionValidationType ConditionValidation { get; set; } = ConditionValidationType.NotExecuted;

        /// <summary>
        /// Gets or sets the condition validation text.
        /// </summary>
        [JsonProperty("ConditionValidation")]
        public string ConditionValidationText
        {
            get => ConditionValidation.ToString();
            set => ConditionValidation = (ConditionValidationType)Enum.Parse(typeof(ConditionValidationType), value);
        }

        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{CheckerName}: {ConditionValidation}";
        }
    }
}