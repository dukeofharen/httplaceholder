using System.Threading.Tasks;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.Interfaces.Signalling;

/// <summary>
/// Describes a class that is used to send messages to a scenario hub.
/// </summary>
public interface IScenarioNotify
{
    /// <summary>
    /// A method to call after a scenario has been set.
    /// </summary>
    /// <param name="scenario">The scenario.</param>
    Task ScenarioSetAsync(ScenarioStateModel scenario);

    /// <summary>
    /// A method to call after a scenario has been deleted.
    /// </summary>
    /// <param name="scenarioName">The scenario name.</param>
    Task ScenarioDeletedAsync(string scenarioName);

    /// <summary>
    /// A method to call after all scenarios have been deleted.
    /// </summary>
    /// <returns></returns>
    Task AllScenariosDeletedAsync();
}
