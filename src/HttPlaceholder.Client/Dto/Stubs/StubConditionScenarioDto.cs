namespace HttPlaceholder.Client.Dto.Stubs
{
    /// <summary>
    /// A model for storing all scenario conditions for a stub
    /// </summary>
    public class StubConditionScenarioDto
    {
        /// <summary>
        /// Gets or sets the inclusive min hit count.
        /// </summary>
        public int? MinHits { get; set; }

        /// <summary>
        /// Gets or sets the exclusive max hit count.
        /// </summary>
        public int? MaxHits { get; set; }

        /// <summary>
        /// Gets or sets the exact hits count.
        /// </summary>
        public int? ExactHits { get; set; }

        /// <summary>
        /// Gets or sets the state the scenario should be in.
        /// </summary>
        public string ScenarioState { get; set; }
    }
}
