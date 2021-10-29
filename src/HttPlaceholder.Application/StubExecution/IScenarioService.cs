namespace HttPlaceholder.Application.StubExecution
{
    /// <summary>
    /// Describes a class that is used for working with scenarios and scenario state.
    /// </summary>
    public interface IScenarioService
    {
        /// <summary>
        /// Increases the hit count of a specific scenario.
        /// </summary>
        /// <param name="scenario"></param>
        void IncreaseHitCount(string scenario);
    }
}
