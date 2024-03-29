﻿using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to match all stubs against an incoming request.
/// </summary>
public interface IStubRequestExecutor
{
    /// <summary>
    ///     Match all stubs against an incoming request.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response to return to the client.</returns>
    Task<ResponseModel> ExecuteRequestAsync(CancellationToken cancellationToken);
}
