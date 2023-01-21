using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model for storing all scenario conditions for a stub response.
/// </summary>
public class StubResponseScenarioDto : IMapFrom<StubResponseScenarioModel>
{
    /// <summary>
    ///     Gets or sets the scenario state the scenario should be set to after the stub is hit.
    /// </summary>
    [YamlMember(Alias = "setScenarioState")]
    public string SetScenarioState { get; set; }

    /// <summary>
    ///     Gets or sets a value which indicates if the state (scenario state and hit count) should be reset after the stub is
    ///     hit.
    /// </summary>
    [YamlMember(Alias = "clearState")]
    public bool? ClearState { get; set; }
}
