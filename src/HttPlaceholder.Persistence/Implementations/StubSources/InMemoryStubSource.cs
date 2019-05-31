using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ducode.Essentials.Console;
using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations.StubSources
{
    internal class InMemoryStubSource : IWritableStubSource
    {
        private static object _lock = new object();

        private readonly SettingsModel _settings;

        internal readonly IList<RequestResultModel> _requestResultModels = new List<RequestResultModel>();
        internal readonly IList<StubModel> _stubModels = new List<StubModel>();

        public InMemoryStubSource(IOptions<SettingsModel> options)
        {
            _settings = options.Value;
        }

        public Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            lock (_lock)
            {
                _requestResultModels.Add(requestResult);
                return Task.CompletedTask;
            }
        }

        public Task AddStubAsync(StubModel stub)
        {
            lock (_lock)
            {
                _stubModels.Add(stub);
                return Task.CompletedTask;
            }
        }

        public Task DeleteAllRequestResultsAsync()
        {
            lock (_lock)
            {
                _requestResultModels.Clear();
                return Task.CompletedTask;
            }
        }

        public Task<bool> DeleteStubAsync(string stubId)
        {
            lock (_lock)
            {
                var stub = _stubModels.FirstOrDefault(s => s.Id == stubId);
                if (stub == null)
                {
                    return Task.FromResult(false);
                }

                _stubModels.Remove(stub);
                return Task.FromResult(true);
            }
        }

        public Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_requestResultModels.AsEnumerable());
            }
        }

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_stubModels.AsEnumerable());
            }
        }

        public Task CleanOldRequestResultsAsync()
        {
            lock (_lock)
            {
                int maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
                var requests = _requestResultModels
                   .OrderByDescending(r => r.RequestEndTime)
                   .Skip(maxLength);
                foreach (var request in requests)
                {
                    _requestResultModels.Remove(request);
                }

                return Task.CompletedTask;
            }
        }

        public Task PrepareStubSourceAsync()
        {
            return Task.CompletedTask;
        }
    }
}
