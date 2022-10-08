using System.Threading;
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
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ScenarioSetAsync(ScenarioStateModel scenario, CancellationToken cancellationToken);

    /// <summary>
    /// A method to call after a scenario has been deleted.
    /// </summary>
    /// <param name="scenarioName">The scenario name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ScenarioDeletedAsync(string scenarioName, CancellationToken cancellationToken);

    /// <summary>
    /// A method to call after all scenarios have been deleted.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AllScenariosDeletedAsync(CancellationToken cancellationToken);
}
