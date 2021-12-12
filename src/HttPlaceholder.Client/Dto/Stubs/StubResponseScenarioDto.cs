namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
/// A model for storing all scenario conditions for a stub response.
/// </summary>
public class StubResponseScenarioDto
{
    /// <summary>
    /// Gets or sets the scenario state the scenario should be set to after the stub is hit.
    /// </summary>
    public string SetScenarioState { get; set; }

    /// <summary>
    /// Gets or sets a value which indicates if the state (scenario state and hit count) should be reset after the stub is hit.
    /// </summary>
    public bool? ClearState { get; set; }
}