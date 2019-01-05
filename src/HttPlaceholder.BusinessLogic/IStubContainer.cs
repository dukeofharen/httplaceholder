using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic
{
    public interface IStubContainer
    {
        Task<IEnumerable<FullStubModel>> GetStubsAsync();

        Task<IEnumerable<FullStubModel>> GetStubsAsync(string tenant);

        Task AddStubAsync(StubModel stub);

        Task<bool> DeleteStubAsync(string stubId);

        Task DeleteAllStubsAsync(string tenant);

        Task UpdateAllStubs(string tenant, IEnumerable<StubModel> stubs);

        Task<FullStubModel> GetStubAsync(string stubId);

        Task AddRequestResultAsync(RequestResultModel requestResult);

        Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();

        Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId);

        Task DeleteAllRequestResultsAsync();

        Task PrepareAsync();
    }
}