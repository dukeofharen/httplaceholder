using System.Collections.Generic;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing all execution related data for a given stub.
/// </summary>
public class StubExecutionResultModel
{
    /// <summary>
    ///     Gets or sets the stub identifier.
    /// </summary>
    public string StubId { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="StubExecutionResultModel" /> is passed.
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    ///     Gets or sets the conditions.
    /// </summary>
    public IEnumerable<ConditionCheckResultModel> Conditions { get; set; }
}
