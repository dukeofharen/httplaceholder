using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is used to convert a <see cref="HttpRequestModel"/> to <see cref="StubConditionsModel"/>.
/// To perform the mapping, the <see cref="IRequestToStubConditionsHandler"/> implementations are used.
/// </summary>
public interface IHttpRequestToConditionsService
{
    /// <summary>
    /// Converts a <see cref="HttpRequestModel"/> to <see cref="StubConditionsModel"/>.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="StubConditionsModel"/>.</returns>
    Task<StubConditionsModel> ConvertToConditionsAsync(HttpRequestModel request, CancellationToken cancellationToken);
}
