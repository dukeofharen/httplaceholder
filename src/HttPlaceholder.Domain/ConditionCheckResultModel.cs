using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing a condition check result.
/// </summary>
public class ConditionCheckResultModel
{
    /// <summary>
    ///     Gets or sets the name of the checker.
    /// </summary>
    public string CheckerName { get; set; }

    /// <summary>
    ///     Gets or sets the condition validation.
    /// </summary>
    public ConditionValidationType ConditionValidation { get; set; } = ConditionValidationType.NotExecuted;

    /// <summary>
    ///     Gets or sets the log.
    /// </summary>
    public string Log { get; set; }
}
