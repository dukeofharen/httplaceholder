using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
/// A stub source that is used to store and read data from a relational database.
/// </summary>
internal class RelationalDbStubSource : IWritableStubSource
{
    // TODO move to separate constants class.
    private const string StubJsonType = "json";

    private readonly SettingsModel _settings;
    private readonly IQueryStore _queryStore;
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IRelationalDbStubCache _relationalDbStubCache;

    public RelationalDbStubSource(
        IOptions<SettingsModel> options,
        IQueryStore queryStore,
        IDatabaseContextFactory databaseContextFactory,
        IRelationalDbStubCache relationalDbStubCache)
    {
        _settings = options.Value;
        _queryStore = queryStore;
        _databaseContextFactory = databaseContextFactory;
        _relationalDbStubCache = relationalDbStubCache;
    }

    /// <inheritdoc />
    public async Task AddRequestResultAsync(RequestResultModel requestResult)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var json = JsonConvert.SerializeObject(requestResult);
        await ctx.ExecuteAsync(_queryStore.AddRequestQuery,
            new
            {
                requestResult.CorrelationId,
                requestResult.ExecutingStubId,
                requestResult.RequestBeginTime,
                requestResult.RequestEndTime,
                Json = json
            });
    }

    /// <inheritdoc />
    public async Task AddStubAsync(StubModel stub)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var json = JsonConvert.SerializeObject(stub);
        await ctx.ExecuteAsync(_queryStore.AddStubQuery,
            new {StubId = stub.Id, Stub = json, StubType = StubJsonType});
        _relationalDbStubCache.ClearStubCache(ctx);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteRequestAsync(string correlationId)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var updatedRows = await ctx.ExecuteAsync(_queryStore.DeleteRequestQuery, new{CorrelationId = correlationId});
        return updatedRows > 0;
    }

    /// <inheritdoc />
    public async Task CleanOldRequestResultsAsync()
    {
        var maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        await ctx.ExecuteAsync(_queryStore.CleanOldRequestsQuery, new {Limit = maxLength});
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync()
    {
        // This method is not optimized right now.
        var requests = await GetRequestResultsAsync();
        return requests.Select(r => new RequestOverviewModel
        {
            Method = r.RequestParameters.Method,
            Url = r.RequestParameters.Url,
            CorrelationId = r.CorrelationId,
            StubTenant = r.StubTenant,
            ExecutingStubId = r.ExecutingStubId,
            RequestBeginTime = r.RequestBeginTime,
            RequestEndTime = r.RequestEndTime
        }).ToArray();
    }

    /// <inheritdoc />
    public async Task<RequestResultModel> GetRequestAsync(string correlationId)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var result = await ctx.QueryFirstOrDefaultAsync<DbRequestModel>(
            _queryStore.GetRequestQuery,
            new {CorrelationId = correlationId});
        return result == null ? null : JsonConvert.DeserializeObject<RequestResultModel>(result.Json);
    }

    /// <inheritdoc />
    public async Task DeleteAllRequestResultsAsync()
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        await ctx.ExecuteAsync(_queryStore.DeleteAllRequestsQuery);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteStubAsync(string stubId)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var updated = await ctx.ExecuteAsync(_queryStore.DeleteStubQuery, new {StubId = stubId});
        _relationalDbStubCache.ClearStubCache(ctx);
        return updated > 0;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var result = await ctx.QueryAsync<DbRequestModel>(_queryStore.GetRequestsQuery);
        return result
            .Select(r => JsonConvert.DeserializeObject<RequestResultModel>(r.Json));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubModel>> GetStubsAsync()
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        return await _relationalDbStubCache.GetOrUpdateStubCache(ctx);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync() =>
        (await GetStubsAsync())
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
        .ToArray();

    /// <inheritdoc />
    public async Task<StubModel> GetStubAsync(string stubId)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var stubs = await _relationalDbStubCache.GetOrUpdateStubCache(ctx);
        return stubs.FirstOrDefault(s => s.Id == stubId);
    }

    /// <inheritdoc />
    public async Task PrepareStubSourceAsync()
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        await ctx.ExecuteAsync(_queryStore.MigrationsQuery);

        // Also initialize the cache at startup.
        await _relationalDbStubCache.GetOrUpdateStubCache(ctx);
    }
}
