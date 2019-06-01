using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Persistence.Implementations
{
    internal class StubContext : IStubContext
    {
        private readonly IEnumerable<IStubSource> _stubSources;

        public StubContext(IEnumerable<IStubSource> stubSources)
        {
            _stubSources = stubSources;
        }

        public async Task<IEnumerable<FullStubModel>> GetStubsAsync() =>
            await GetStubsAsync(false);

        public async Task<IEnumerable<FullStubModel>> GetStubsAsync(string tenant)
        {
            var stubs = await GetStubsAsync(false);
            return stubs
                .Where(s => string.Equals(s.Stub.Tenant, tenant, StringComparison.OrdinalIgnoreCase));
        }

        public async Task AddStubAsync(StubModel stub)
        {
            if (string.IsNullOrWhiteSpace(stub.Id))
            {
                // If no ID is sent, create one here.
                stub.Id = Guid.NewGuid().ToString();
            }

            // Check that a stub with the new ID isn't already added to a readonly stub source.
            var stubs = await GetStubsAsync(true);
            if (stubs.Any(s => string.Equals(stub.Id, s.Stub.Id, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException($"Stub with ID '{stub.Id}'.");
            }

            var source = GetWritableStubSource();
            await source.AddStubAsync(stub);
        }

        public async Task<bool> DeleteStubAsync(string stubId) =>
            await GetWritableStubSource().DeleteStubAsync(stubId);

        public async Task DeleteAllStubsAsync(string tenant)
        {
            var source = GetWritableStubSource();
            var stubs = (await source.GetStubsAsync())
                .Where(s => string.Equals(s.Tenant, tenant, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            foreach (var stub in stubs)
            {
                await source.DeleteStubAsync(stub.Id);
            }
        }

        public async Task UpdateAllStubs(string tenant, IEnumerable<StubModel> stubs)
        {
            var source = GetWritableStubSource();
            var stubIds = stubs
                .Select(s => s.Id)
                .Distinct();
            var existingStubs = (await source.GetStubsAsync())
                .Where(s => stubIds.Any(sid => string.Equals(sid, s.Id, StringComparison.OrdinalIgnoreCase)) ||
                            string.Equals(s.Tenant, tenant, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            // First, delete the existing stubs.
            foreach (var stub in existingStubs)
            {
                await source.DeleteStubAsync(stub.Id);
            }

            // Make sure the new selection of stubs all have the new tenant and add them to the stub source.
            foreach (var stub in stubs)
            {
                stub.Tenant = tenant;
                await source.AddStubAsync(stub);
            }
        }

        public async Task<FullStubModel> GetStubAsync(string stubId) =>
            (await GetStubsAsync()).FirstOrDefault(s => s.Stub.Id == stubId);

        public async Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            var source = GetWritableStubSource();

            // Clean up old requests here.
            await source.CleanOldRequestResultsAsync();

            await source.AddRequestResultAsync(requestResult);
        }

        public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync() =>
            (await GetWritableStubSource().GetRequestResultsAsync())
               .OrderByDescending(s => s.RequestBeginTime);

        public async Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId)
        {
            var source = GetWritableStubSource();
            var results = await source.GetRequestResultsAsync();
            results = results
               .Where(r => r.ExecutingStubId == stubId)
               .OrderByDescending(s => s.RequestBeginTime);
            return results;
        }

        public async Task DeleteAllRequestResultsAsync()
        {
            var source = GetWritableStubSource();
            await source.DeleteAllRequestResultsAsync();
        }

        public async Task PrepareAsync()
        {
            foreach (var source in _stubSources)
            {
                // Call PrepareStubSourceAsync on all stub sources for letting them prepare their own setup (e.g. create tables, folders etc.).
                await source.PrepareStubSourceAsync();
            }
        }

        private IWritableStubSource GetWritableStubSource() =>
            (IWritableStubSource)_stubSources.Single(s => s is IWritableStubSource);

        private IEnumerable<IStubSource> GetReadOnlyStubSources() =>
            _stubSources.Where(s => !(s is IWritableStubSource));

        private async Task<IEnumerable<FullStubModel>> GetStubsAsync(bool readOnly)
        {
            var result = new List<FullStubModel>();
            foreach (var source in readOnly ? GetReadOnlyStubSources() : _stubSources)
            {
                var stubSourceIsReadOnly = !(source is IWritableStubSource);
                var stubs = await source.GetStubsAsync();
                var fullStubModels = stubs.Select(s => new FullStubModel
                {
                    Stub = s,
                    Metadata = new StubMetadataModel
                    {
                        ReadOnly = stubSourceIsReadOnly
                    }
                });
                result.AddRange(fullStubModels);
            }

            return result;
        }
    }
}
