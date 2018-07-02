using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Placeholder.Models;
using Placeholder.Services;
using Placeholder.Utilities;

namespace Placeholder.DataLogic.Implementations.StubSources
{
   internal class InMemoryStubSource : IWritableStubSource
   {
      private readonly IConfigurationService _configurationService;

      internal readonly IList<RequestResultModel> _requestResultModels = new List<RequestResultModel>();
      internal readonly IList<StubModel> _stubModels = new List<StubModel>();

      public InMemoryStubSource(IConfigurationService configurationService)
      {
         _configurationService = configurationService;
      }

      public Task AddRequestResultAsync(RequestResultModel requestResult)
      {
         CleanOldRequestResults();
         _requestResultModels.Add(requestResult);
         return Task.CompletedTask;
      }

      public Task AddStubAsync(StubModel stub)
      {
         _stubModels.Add(stub);
         return Task.CompletedTask;
      }

      public Task DeleteAllRequestResultsAsync()
      {
         _requestResultModels.Clear();
         return Task.CompletedTask;
      }

      public Task<bool> DeleteStubAsync(string stubId)
      {
         var stub = _stubModels.FirstOrDefault(s => s.Id == stubId);
         if (stub == null)
         {
            return Task.FromResult(false);
         }

         _stubModels.Remove(stub);
         return Task.FromResult(true);
      }

      public Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
      {
         return Task.FromResult(_requestResultModels.AsEnumerable());
      }

      public Task<IEnumerable<StubModel>> GetStubsAsync()
      {
         return Task.FromResult(_stubModels.AsEnumerable());
      }

      private void CleanOldRequestResults()
      {
         var config = _configurationService.GetConfiguration();
         int maxLength = config.GetValue(Constants.ConfigKeys.OldRequestsQueueLengthKey, 40);

         var requests = _requestResultModels
            .OrderByDescending(r => r.RequestEndTime)
            .Skip(maxLength);
         foreach (var request in requests)
         {
            _requestResultModels.Remove(request);
         }
      }
   }
}
