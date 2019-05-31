using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces
{
    public interface IStubSource
    {
        Task<IEnumerable<StubModel>> GetStubsAsync();

        Task PrepareStubSourceAsync();
    }
}
