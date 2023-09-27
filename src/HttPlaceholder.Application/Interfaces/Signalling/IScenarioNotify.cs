using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.Interfaces.Signalling;

/// <summary>
///     Describes a class that is used to send messages to a scenario hub.
/// </summary>
public interface IScenarioNotify
{
    /// <summary>
    ///     A method to call after a scenario has been set.
    /// </summary>
    /// <param name="scenario">The scenario.</param>
    /// <param name="distributionKey">The distribution key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ScenarioSetAsync(ScenarioStateModel scenario, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A method to call after a scenario has been deleted.
    /// </summary>
    /// <param name="scenarioName">The scenario name.</param>
    /// <param name="distributionKey">The distribution key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ScenarioDeletedAsync(string scenarioName, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A method to call after all scenarios have been deleted.
    /// </summary>
    /// <param name="distributionKey">The distribution key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AllScenariosDeletedAsync(string distributionKey = null, CancellationToken cancellationToken = default);
}
