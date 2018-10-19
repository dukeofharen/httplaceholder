using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.DataLogic;
using HttPlaceholder.Exceptions;
using HttPlaceholder.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.BusinessLogic.Implementations
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
            return await GetStubsAsync(false);
        }

        public async Task<IEnumerable<StubModel>> GetStubsAsync(string tenant)
        {
            var stubs = await GetStubsAsync(false);
            var stubsForTenant = stubs
                .Where(s => string.Equals(s.Tenant, tenant, StringComparison.OrdinalIgnoreCase));
            return stubsForTenant;
        }

        public async Task AddStubAsync(StubModel stub)
        {
            // Check that a stub with the new ID isn't already added to a readonly stub source.
            var stubs = await GetStubsAsync(true);
            if (stubs.Any(s => string.Equals(stub.Id, s.Id, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException($"Stub with ID '{stub.Id}'.");
            }

            var source = GetWritableStubSource();
            await source.AddStubAsync(stub);
        }

        public async Task<bool> DeleteStubAsync(string stubId)
        {
            var source = GetWritableStubSource();
            return await source.DeleteStubAsync(stubId);
        }

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

        public async Task<StubModel> GetStubAsync(string stubId)
        {
            var stub = (await GetStubsAsync()).FirstOrDefault(s => s.Id == stubId);
            return stub;
        }

        public async Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            var source = GetWritableStubSource();

            // Clean up old requests here.
            await source.CleanOldRequestResultsAsync();

            await source.AddRequestResultAsync(requestResult);
        }

        public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
        {
            var source = GetWritableStubSource();
            return (await source.GetRequestResultsAsync())
               .OrderByDescending(s => s.RequestBeginTime);
        }

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

        private IWritableStubSource GetWritableStubSource()
        {
            var sources = GetStubSources();
            var writableStubSource = (IWritableStubSource)sources.Single(s => s is IWritableStubSource);
            return writableStubSource;
        }

        private IEnumerable<IStubSource> GetReadOnlyStubSources()
        {
            var sources = GetStubSources();
            var readOnlyStubSources = sources.Where(s => !(s is IWritableStubSource));
            return readOnlyStubSources;
        }

        private IEnumerable<IStubSource> GetStubSources()
        {
            var sources = ((IEnumerable<IStubSource>)_serviceProvider.GetServices(typeof(IStubSource))).ToArray();
            return sources;
        }

        private async Task<IEnumerable<StubModel>> GetStubsAsync(bool readOnly)
        {
            var result = new List<StubModel>();
            var sources = readOnly ? GetReadOnlyStubSources() : GetStubSources();
            foreach (var source in sources)
            {
                result.AddRange(await source.GetStubsAsync());
            }

            return result;
        }
    }
}