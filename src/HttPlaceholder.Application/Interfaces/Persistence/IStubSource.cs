using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Persistence;

public interface IStubSource
{
    Task<IEnumerable<StubModel>> GetStubsAsync();

    Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync();

    Task<StubModel> GetStubAsync(string stubId);

    Task PrepareStubSourceAsync();
}