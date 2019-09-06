using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    public interface IStubContext
    {
        Task<IEnumerable<FullStubModel>> GetStubsAsync();

        Task<IEnumerable<FullStubModel>> GetStubsAsync(string tenant);

        Task<FullStubModel> AddStubAsync(StubModel stub);

        Task<bool> DeleteStubAsync(string stubId);

        Task DeleteAllStubsAsync(string tenant);

        Task DeleteAllStubsAsync();

        Task UpdateAllStubs(string tenant, IEnumerable<StubModel> stubs);

        Task<FullStubModel> GetStubAsync(string stubId);

        Task AddRequestResultAsync(RequestResultModel requestResult);

        Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();

        Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId);

        Task DeleteAllRequestResultsAsync();

        Task<IEnumerable<string>> GetTenantsAsync();

        Task PrepareAsync();
    }
}
