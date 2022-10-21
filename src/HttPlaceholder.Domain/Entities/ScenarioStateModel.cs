namespace HttPlaceholder.Domain.Entities;

/// <summary>
///     Represents the state of a specific scenario.
/// </summary>
public class ScenarioStateModel
{
    /// <summary>
    ///     Constructs a <see cref="ScenarioStateModel" /> instance.
    /// </summary>
    public ScenarioStateModel()
    {
    }

    /// <summary>
    ///     Constructs a <see cref="ScenarioStateModel" /> instance.
    /// </summary>
    /// <param name="scenario">The scenario.</param>
    public ScenarioStateModel(string scenario)
    {
        Scenario = scenario.ToLower();
    }

    /// <summary>
    ///     Gets or sets the scenario name.
    /// </summary>
    public string Scenario { get; set; }

    /// <summary>
    ///     Gets or sets the state the scenario is in.
    /// </summary>
    public string State { get; set; } = Constants.DefaultScenarioState;

    /// <summary>
    ///     Gets or sets the number of times the scenario has been hit.
    /// </summary>
    public int HitCount { get; set; }
}
