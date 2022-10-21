namespace HttPlaceholder.Client.Dto.Enums;

/// <summary>
///     A utility enum to determine the priority for a stub.
/// </summary>
public enum PriorityType
{
    /// <summary>
    ///     The default priority.
    /// </summary>
    Default = 0,

    /// <summary>
    ///     Low priority.
    /// </summary>
    Low = -1,

    /// <summary>
    ///     Medium priority.
    /// </summary>
    Medium = 5,

    /// <summary>
    ///     High priority.
    /// </summary>
    High = 10
}
