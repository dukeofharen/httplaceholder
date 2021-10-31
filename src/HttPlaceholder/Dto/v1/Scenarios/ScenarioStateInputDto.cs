using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Dto.v1.Scenarios
{
    /// <summary>
    /// A model that is used to set the scenario.
    /// </summary>
    public class ScenarioStateInputDto : IMapTo<ScenarioStateModel>
    {
        /// <summary>
        /// Gets or sets the state the scenario is in.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the number of times the scenario has been hit.
        /// </summary>
        public int HitCount { get; set; }
    }
}
