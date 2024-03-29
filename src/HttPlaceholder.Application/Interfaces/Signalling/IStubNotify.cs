﻿using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Signalling;

/// <summary>
///     Describes a class that is used to send messages to a stub hub.
/// </summary>
public interface IStubNotify
{
    /// <summary>
    ///     Handle the addition of a new stub.
    /// </summary>
    /// <param name="stub">The stub.</param>
    /// <param name="distributionKey">The distribution key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task StubAddedAsync(FullStubOverviewModel stub, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handle the deletion of a stub.
    /// </summary>
    /// <param name="stubId">The ID of the deleted stub.</param>
    /// <param name="distributionKey">The distribution key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task StubDeletedAsync(string stubId, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Signal that the stubs should be reloaded.
    /// </summary>
    /// <param name="distributionKey">The distribution key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ReloadStubsAsync(string distributionKey = null, CancellationToken cancellationToken = default);
}
