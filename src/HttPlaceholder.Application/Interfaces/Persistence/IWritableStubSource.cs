using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Persistence;

public interface IWritableStubSource : IStubSource
{
    Task AddStubAsync(StubModel stub);

    Task<bool> DeleteStubAsync(string stubId);

    Task AddRequestResultAsync(RequestResultModel requestResult);

    Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();

    Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync();

    Task<RequestResultModel> GetRequestAsync(string correlationId);

    Task DeleteAllRequestResultsAsync();

    Task<bool> DeleteRequestAsync(string correlationId);

    Task CleanOldRequestResultsAsync();
}