using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class HttpResponseToStubResponseService : IHttpResponseToStubResponseService
{
    /// <inheritdoc />
    public Task<StubResponseModel> ConvertToResponseAsync(HttpResponseModel response) =>
        throw new System.NotImplementedException();
}
