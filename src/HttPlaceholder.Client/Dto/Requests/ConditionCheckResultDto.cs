namespace HttPlaceholder.Client.Dto.Requests
{
    /// <summary>
    /// A model for storing a condition check result.
    /// </summary>
    public class ConditionCheckResultDto
    {
        /// <summary>
        /// Gets or sets the name of the checker.
        /// </summary>
        public string CheckerName { get; set; }

        /// <summary>
        /// Gets or sets the condition validation.
        /// </summary>
        public string ConditionValidation { get; set; }
    }
}
