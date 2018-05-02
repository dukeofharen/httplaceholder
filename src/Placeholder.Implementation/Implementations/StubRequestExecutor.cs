using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   internal class StubRequestExecutor : IStubRequestExecutor
   {
      public Task<ResponseModel> ExecuteRequestAsync()
      {
         return Task.FromResult(new ResponseModel
         {
            StatusCode = HttpStatusCode.Conflict,
            Body = "JA MOI EEM!",
            Headers = new Dictionary<string, string>
             {
                {"Server", "TestServer"}
             }
         });
      }
   }
}
