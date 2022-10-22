using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Dto.v1.Scenarios;

/// <summary>
///     Represents the state of a specific scenario.
/// </summary>
public class ScenarioStateDto : IMapFrom<ScenarioStateModel>, IMapTo<ScenarioStateModel>
{
    /// <summary>
    ///     Gets or sets the scenario name.
    /// </summary>
    public string Scenario { get; set; }

    /// <summary>
    ///     Gets or sets the state the scenario is in.
    /// </summary>
    public string State { get; set; }

    /// <summary>
    ///     Gets or sets the number of times the scenario has been hit.
    /// </summary>
    public int HitCount { get; set; }
}
