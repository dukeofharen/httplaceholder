namespace HttPlaceholder.Client.Dto.Scenarios
{
    /// <summary>
    /// A model that is used to set the scenario.
    /// </summary>
    public class ScenarioStateInputDto
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