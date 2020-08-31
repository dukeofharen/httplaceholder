﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations.StubSources
{
    internal class InMemoryStubSource : IWritableStubSource
    {
        private static readonly object _lock = new object();

        private readonly SettingsModel _settings;

        internal readonly IList<RequestResultModel> RequestResultModels = new List<RequestResultModel>();
        internal readonly IList<StubModel> StubModels = new List<StubModel>();

        public InMemoryStubSource(IOptions<SettingsModel> options)
        {
            _settings = options.Value;
        }

        public Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            lock (_lock)
            {
                RequestResultModels.Add(requestResult);
                return Task.CompletedTask;
            }
        }

        public Task AddStubAsync(StubModel stub)
        {
            lock (_lock)
            {
                StubModels.Add(stub);
                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync() => throw new System.NotImplementedException();

        public Task<RequestResultModel> GetRequestAsync(string correlationId)
        {
            lock (_lock)
            {
                return Task.FromResult(RequestResultModels.FirstOrDefault(r => r.CorrelationId == correlationId));
            }
        }

        public Task DeleteAllRequestResultsAsync()
        {
            lock (_lock)
            {
                RequestResultModels.Clear();
                return Task.CompletedTask;
            }
        }

        public Task<bool> DeleteStubAsync(string stubId)
        {
            lock (_lock)
            {
                var stub = StubModels.FirstOrDefault(s => s.Id == stubId);
                if (stub == null)
                {
                    return Task.FromResult(false);
                }

                StubModels.Remove(stub);
                return Task.FromResult(true);
            }
        }

        public Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(RequestResultModels.AsEnumerable());
            }
        }

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(StubModels.AsEnumerable());
            }
        }

        public Task CleanOldRequestResultsAsync()
        {
            lock (_lock)
            {
                var maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
                var requests = RequestResultModels
                    .OrderByDescending(r => r.RequestEndTime)
                    .Skip(maxLength);
                foreach (var request in requests)
                {
                    RequestResultModels.Remove(request);
                }

                return Task.CompletedTask;
            }
        }

        public Task PrepareStubSourceAsync() => Task.CompletedTask;
    }
}
