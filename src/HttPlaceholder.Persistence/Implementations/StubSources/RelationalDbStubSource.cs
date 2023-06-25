using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to store and read data from a relational database.
/// </summary>
internal class RelationalDbStubSource : BaseWritableStubSource
{
    private readonly IDatabaseContextFactory _databaseContextFactory;

    private readonly IOptionsMonitor<SettingsModel> _options;
    private readonly IQueryStore _queryStore;
    private readonly IRelationalDbMigrator _relationalDbMigrator;
    private readonly IRelationalDbStubCache _relationalDbStubCache;

    public RelationalDbStubSource(
        IOptionsMonitor<SettingsModel> options,
        IQueryStore queryStore,
        IDatabaseContextFactory databaseContextFactory,
        IRelationalDbStubCache relationalDbStubCache,
        IRelationalDbMigrator relationalDbMigrator)
    {
        _options = options;
        _queryStore = queryStore;
        _databaseContextFactory = databaseContextFactory;
        _relationalDbStubCache = relationalDbStubCache;
        _relationalDbMigrator = relationalDbMigrator;
    }

    /// <inheritdoc />
    public override async Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var hasResponse = responseModel != null;
        requestResult.HasResponse = hasResponse;
        var json = JsonConvert.SerializeObject(requestResult);
        await ctx.ExecuteAsync(_queryStore.AddRequestQuery,
            cancellationToken,
            new
            {
                requestResult.CorrelationId,
                requestResult.ExecutingStubId,
                requestResult.RequestBeginTime,
                requestResult.RequestEndTime,
                Json = json,
                HasResponse = hasResponse
            });
        if (hasResponse)
        {
            await ctx.ExecuteAsync(_queryStore.AddResponseQuery,
                cancellationToken,
                new
                {
                    requestResult.CorrelationId,
                    responseModel.StatusCode,
                    Headers = JsonConvert.SerializeObject(responseModel.Headers),
                    Body = responseModel.Body != null ? Convert.ToBase64String(responseModel.Body) : string.Empty,
                    responseModel.BodyIsBinary
                });
        }
    }

    /// <inheritdoc />
    public override async Task AddStubAsync(StubModel stub, CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var json = JsonConvert.SerializeObject(stub);
        await ctx.ExecuteAsync(_queryStore.AddStubQuery,
            cancellationToken,
            new {StubId = stub.Id, Stub = json, StubType = StubTypes.StubJsonType});
        await _relationalDbStubCache.AddOrReplaceStubAsync(ctx, stub, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var updatedRows = await ctx.ExecuteAsync(_queryStore.DeleteRequestQuery, cancellationToken,
            new {CorrelationId = correlationId});
        return updatedRows > 0;
    }

    /// <inheritdoc />
    public override async Task CleanOldRequestResultsAsync(CancellationToken cancellationToken)
    {
        var maxLength = _options.CurrentValue.Storage?.OldRequestsQueueLength ?? 40;
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        await ctx.ExecuteAsync(_queryStore.CleanOldRequestsQuery, cancellationToken, new {Limit = maxLength});
    }

    /// <inheritdoc />
    public override async Task<RequestResultModel> GetRequestAsync(string correlationId,
        CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var result = await ctx.QueryFirstOrDefaultAsync<DbRequestModel>(
            _queryStore.GetRequestQuery,
            cancellationToken,
            new {CorrelationId = correlationId});
        return result == null ? null : JsonConvert.DeserializeObject<RequestResultModel>(result.Json);
    }

    /// <inheritdoc />
    public override async Task<ResponseModel> GetResponseAsync(string correlationId,
        CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var result = await ctx.QueryFirstOrDefaultAsync<DbResponseModel>(
            _queryStore.GetResponseQuery,
            cancellationToken,
            new {CorrelationId = correlationId});
        if (result == null)
        {
            return null;
        }

        return new ResponseModel
        {
            Body = Convert.FromBase64String(result.Body),
            Headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Headers),
            StatusCode = result.StatusCode,
            BodyIsBinary = result.BodyIsBinary
        };
    }

    /// <inheritdoc />
    public override async Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        await ctx.ExecuteAsync(_queryStore.DeleteAllRequestsQuery, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var updated = await ctx.ExecuteAsync(_queryStore.DeleteStubQuery, cancellationToken, new {StubId = stubId});
        await _relationalDbStubCache.DeleteStubAsync(ctx, stubId, cancellationToken);
        return updated > 0;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(
        PagingModel pagingModel,
        CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        IEnumerable<DbRequestModel> result;
        if (!string.IsNullOrWhiteSpace(pagingModel?.FromIdentifier))
        {
            var correlationIds =
                (await ctx.QueryAsync<string>(_queryStore.GetPagedRequestCorrelationIdsQuery, cancellationToken))
                .ToArray();
            var index = correlationIds
                .Select((correlationId, index) => new {correlationId, index})
                .Where(x => x.correlationId.Equals(pagingModel.FromIdentifier))
                .Select(f => f.index)
                .FirstOrDefault();
            correlationIds = correlationIds.Skip(index).ToArray();
            if (pagingModel.ItemsPerPage.HasValue)
            {
                correlationIds = correlationIds.Take(pagingModel.ItemsPerPage.Value).ToArray();
            }

            result = await ctx.QueryAsync<DbRequestModel>(_queryStore.GetRequestsByCorrelationIdsQuery,
                cancellationToken, new {CorrelationIds = correlationIds});
        }
        else
        {
            result = await ctx.QueryAsync<DbRequestModel>(_queryStore.GetRequestsQuery, cancellationToken);
        }

        return result
            .Select(r => JsonConvert.DeserializeObject<RequestResultModel>(r.Json));
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<StubModel>> GetStubsAsync(CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        return await _relationalDbStubCache.GetOrUpdateStubCacheAsync(ctx, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(
        CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken))
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled});

    /// <inheritdoc />
    public override async Task<StubModel> GetStubAsync(string stubId, CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        var stubs = await _relationalDbStubCache.GetOrUpdateStubCacheAsync(ctx, cancellationToken);
        return stubs.FirstOrDefault(s => s.Id == stubId);
    }

    /// <inheritdoc />
    public override async Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        using var ctx = _databaseContextFactory.CreateDatabaseContext();
        await _relationalDbMigrator.MigrateAsync(ctx, cancellationToken);

        // Also initialize the cache at startup.
        await _relationalDbStubCache.GetOrUpdateStubCacheAsync(ctx, cancellationToken);
    }
}
