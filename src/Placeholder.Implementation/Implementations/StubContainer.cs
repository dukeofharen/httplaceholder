using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.DataLogic;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   internal class StubContainer : IStubContainer
   {
      private readonly IServiceProvider _serviceProvider;

      public StubContainer(IServiceProvider serviceProvider)
      {
         _serviceProvider = serviceProvider;
      }

      public async Task<IEnumerable<StubModel>> GetStubsAsync()
      {
         var result = new List<StubModel>();
         var sources = GetStubSources();
         foreach(var source in sources)
         {
            result.AddRange(await source.GetStubsAsync());
         }

         return result;
      }

      public async Task AddStubAsync(StubModel stub)
      {
         var source = GetWritableStubSource();
         await source.AddStubAsync(stub);
      }

      public async Task<bool> DeleteStubAsync(string stubId)
      {
         var source = GetWritableStubSource();
         return await source.DeleteStubAsync(stubId);
      }

      public async Task<StubModel> GetStubAsync(string stubId)
      {
         var stub = (await GetStubsAsync()).FirstOrDefault(s => s.Id == stubId);
         return stub;
      }

      public async Task AddRequestResultAsync(RequestResultModel requestResult)
      {
         var source = GetWritableStubSource();
         await source.AddRequestResultAsync(requestResult);
      }

      private IWritableStubSource GetWritableStubSource()
      {
         var sources = GetStubSources();
         var writableStubSource = (IWritableStubSource)sources.Single(s => s is IWritableStubSource);
         return writableStubSource;
      }

      private IEnumerable<IStubSource> GetStubSources()
      {
         var sources = ((IEnumerable<IStubSource>)_serviceProvider.GetServices(typeof(IStubSource))).ToArray();
         return sources;
      }
   }
}
