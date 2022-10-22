namespace HttPlaceholder.Domain.Enums;

/// <summary>
///     An enum for specifying the result of a condition checker.
/// </summary>
public enum ConditionValidationType
{
    /// <summary>
    ///     Not set.
    /// </summary>
    NotSet,

    /// <summary>
    ///     Condition is valid.
    /// </summary>
    Valid,

    /// <summary>
    ///     Condition is invalid.
    /// </summary>
    Invalid,

    /// <summary>
    ///     Condition is not executed.
    /// </summary>
    NotExecuted
}
