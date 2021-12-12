using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Dto.v1.Requests;

/// <summary>
/// A model for storing all execution related data for a given stub.
/// </summary>
public class StubExecutionResultDto : IMapFrom<StubExecutionResultModel>, IMapTo<StubExecutionResultModel>
{
    /// <summary>
    /// Gets or sets the stub identifier.
    /// </summary>
    public string StubId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="StubExecutionResultDto"/> is passed.
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Gets or sets the conditions.
    /// </summary>
    public IEnumerable<ConditionCheckResultDto> Conditions { get; set; }
}