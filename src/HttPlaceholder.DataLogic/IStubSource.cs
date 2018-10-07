using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.DataLogic
{
    public interface IStubSource
    {
        Task<IEnumerable<StubModel>> GetStubsAsync();
    }
}