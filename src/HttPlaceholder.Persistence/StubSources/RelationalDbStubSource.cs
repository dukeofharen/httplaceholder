using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.StubSources;

/// <summary>
///     A stub source that is used to store and read data from a relational database.
/// </summary>
internal class RelationalDbStubSource(
    IOptionsMonitor<SettingsModel> options,
    IQueryStore queryStore,
    IDatabaseContextFactory databaseContextFactory,
    IRelationalDbStubCache relationalDbStubCache,
    IRelationalDbMigrator relationalDbMigrator)
    : BaseWritableStubSource
{
    /// <inheritdoc />
    public override async Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var hasResponse = responseModel != null;
        requestResult.HasResponse = hasResponse;
        var json = JsonConvert.SerializeObject(requestResult);
        await ctx.ExecuteAsync(queryStore.AddRequestQuery,
            cancellationToken,
            new
            {
                requestResult.CorrelationId,
                requestResult.ExecutingStubId,
                requestResult.RequestBeginTime,
                requestResult.RequestEndTime,
                Json = json,
                HasResponse = hasResponse,
                DistributionKey = CleanDistKey(distributionKey)
            });
        if (hasResponse)
        {
            await ctx.ExecuteAsync(queryStore.AddResponseQuery,
                cancellationToken,
                new
                {
                    requestResult.CorrelationId,
                    responseModel.StatusCode,
                    Headers = JsonConvert.SerializeObject(responseModel.Headers),
                    Body = responseModel.Body != null ? Convert.ToBase64String(responseModel.Body) : string.Empty,
                    responseModel.BodyIsBinary,
                    DistributionKey = CleanDistKey(distributionKey)
                });
        }
    }

    /// <inheritdoc />
    public override async Task AddStubAsync(StubModel stub, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var json = JsonConvert.SerializeObject(stub);
        await ctx.ExecuteAsync(queryStore.AddStubQuery,
            cancellationToken,
            new
            {
                StubId = stub.Id,
                Stub = json,
                StubType = StubTypes.StubJsonType,
                DistributionKey = CleanDistKey(distributionKey)
            });
        if (string.IsNullOrWhiteSpace(distributionKey))
        {
            await relationalDbStubCache.AddOrReplaceStubAsync(ctx, stub, cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var updatedRows = await ctx.ExecuteAsync(queryStore.DeleteRequestQuery, cancellationToken,
            new { CorrelationId = correlationId, DistributionKey = CleanDistKey(distributionKey) });
        return updatedRows > 0;
    }

    /// <inheritdoc />
    public override async Task CleanOldRequestResultsAsync(CancellationToken cancellationToken = default)
    {
        var maxLength = options.CurrentValue.Storage?.OldRequestsQueueLength ?? 40;
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var keys = await ctx.QueryAsync<string>(queryStore.GetDistinctRequestDistributionKeysQuery, cancellationToken);
        foreach (var key in keys)
        {
            await ctx.ExecuteAsync(queryStore.CleanOldRequestsQuery, cancellationToken,
                new { Limit = maxLength, DistributionKey = key });
        }
    }

    /// <inheritdoc />
    public override async Task<ScenarioStateModel> GetScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        return await GetScenarioInternalAsync(ctx, scenario, distributionKey, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<ScenarioStateModel> AddScenarioAsync(string scenario,
        ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var existingScenario = await GetScenarioInternalAsync(ctx, scenario, distributionKey, cancellationToken);
        if (existingScenario != null)
        {
            throw new InvalidOperationException(string.Format(PersistenceResources.ScenarioWithKeyAlreadyExists,
                scenario));
        }

        await ctx.ExecuteAsync(queryStore.AddScenarioQuery, cancellationToken,
            new
            {
                DistributionKey = CleanDistKey(distributionKey),
                Scenario = scenario,
                scenarioStateModel.State,
                scenarioStateModel.HitCount
            });
        return scenarioStateModel;
    }

    /// <inheritdoc />
    public override async Task UpdateScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var existingScenario = await GetScenarioInternalAsync(ctx, scenario, distributionKey, cancellationToken);
        if (existingScenario == null)
        {
            throw new InvalidOperationException(string.Format(PersistenceResources.ScenarioWithKeyNotFound, scenario));
        }

        await ctx.ExecuteAsync(queryStore.UpdateScenarioQuery, cancellationToken,
            new
            {
                scenarioStateModel.State,
                scenarioStateModel.HitCount,
                Scenario = scenario,
                DistributionKey = CleanDistKey(distributionKey)
            });
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<ScenarioStateModel>> GetAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        return await ctx.QueryAsync<ScenarioStateModel>(queryStore.GetAllScenariosQuery, cancellationToken,
            new { DistributionKey = CleanDistKey(distributionKey) });
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return false;
        }

        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var count = await ctx.ExecuteAsync(queryStore.DeleteScenarioQuery, cancellationToken,
            new { Scenario = scenario, DistributionKey = CleanDistKey(distributionKey) });
        return count >= 1;
    }

    /// <inheritdoc />
    public override async Task DeleteAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        await ctx.ExecuteAsync(queryStore.DeleteAllScenariosQuery, cancellationToken,
            new { DistributionKey = CleanDistKey(distributionKey) });
    }

    /// <inheritdoc />
    public override async Task<RequestResultModel> GetRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var result = await ctx.QueryFirstOrDefaultAsync<DbRequestModel>(
            queryStore.GetRequestQuery,
            cancellationToken,
            new { CorrelationId = correlationId, DistributionKey = CleanDistKey(distributionKey) });
        return result == null ? null : JsonConvert.DeserializeObject<RequestResultModel>(result.Json);
    }

    /// <inheritdoc />
    public override async Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var result = await ctx.QueryFirstOrDefaultAsync<DbResponseModel>(
            queryStore.GetResponseQuery,
            cancellationToken,
            new { CorrelationId = correlationId, DistributionKey = CleanDistKey(distributionKey) });
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
    public override async Task DeleteAllRequestResultsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        await ctx.ExecuteAsync(queryStore.DeleteAllRequestsQuery, cancellationToken,
            new { DistributionKey = CleanDistKey(distributionKey) });
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var updated = await ctx.ExecuteAsync(queryStore.DeleteStubQuery, cancellationToken,
            new { StubId = stubId, DistributionKey = CleanDistKey(distributionKey) });
        if (string.IsNullOrWhiteSpace(distributionKey))
        {
            await relationalDbStubCache.DeleteStubAsync(ctx, stubId, cancellationToken);
        }

        return updated > 0;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        IEnumerable<DbRequestModel> result;
        if (pagingModel != null)
        {
            IEnumerable<string> correlationIds =
                (await ctx.QueryAsync<string>(queryStore.GetPagedRequestCorrelationIdsQuery, cancellationToken,
                    new { DistributionKey = CleanDistKey(distributionKey) }))
                .ToArray();
            if (!string.IsNullOrWhiteSpace(pagingModel.FromIdentifier))
            {
                var index = correlationIds
                    .Select((correlationId, index) => new { correlationId, index })
                    .Where(x => x.correlationId.Equals(pagingModel.FromIdentifier))
                    .Select(f => f.index)
                    .FirstOrDefault();
                correlationIds = correlationIds.Skip(index);
            }

            if (pagingModel.ItemsPerPage.HasValue)
            {
                correlationIds = correlationIds.Take(pagingModel.ItemsPerPage.Value);
            }

            result = await ctx.QueryAsync<DbRequestModel>(queryStore.GetRequestsByCorrelationIdsQuery,
                cancellationToken,
                new { CorrelationIds = correlationIds.ToArray(), DistributionKey = CleanDistKey(distributionKey) });
        }
        else
        {
            result = await ctx.QueryAsync<DbRequestModel>(queryStore.GetRequestsQuery, cancellationToken,
                new { DistributionKey = CleanDistKey(distributionKey) });
        }

        return result
            .Select(r => JsonConvert.DeserializeObject<RequestResultModel>(r.Json));
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var result = await relationalDbStubCache.GetOrUpdateStubCacheAsync(CleanDistKey(distributionKey), ctx,
            cancellationToken);
        return result.Select(s => (s, new Dictionary<string, string>()));
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>>
        GetStubsOverviewAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => (new StubOverviewModel { Id = s.Stub.Id, Tenant = s.Stub.Tenant, Enabled = s.Stub.Enabled },
            s.Metadata))
        .ToArray();

    /// <inheritdoc />
    public override async Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(string stubId,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        var stubs = await relationalDbStubCache.GetOrUpdateStubCacheAsync(CleanDistKey(distributionKey), ctx,
            cancellationToken);
        var result = stubs.FirstOrDefault(s => s.Id == stubId);
        return result != null ? (result, new Dictionary<string, string>()) : null;
    }

    /// <inheritdoc />
    public override async Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        using var ctx = databaseContextFactory.CreateDatabaseContext();
        await relationalDbMigrator.MigrateAsync(ctx, cancellationToken);

        // Also initialize the cache at startup.
        await relationalDbStubCache.GetOrUpdateStubCacheAsync(string.Empty, ctx, cancellationToken);
    }

    private static string CleanDistKey(string distributionKey) => distributionKey ?? string.Empty;

    private async Task<ScenarioStateModel> GetScenarioInternalAsync(IDatabaseContext ctx, string scenario,
        string distributionKey, CancellationToken cancellationToken) =>
        await ctx.QueryFirstOrDefaultAsync<ScenarioStateModel>(queryStore.GetScenarioQuery,
            cancellationToken, new { Scenario = scenario, DistributionKey = CleanDistKey(distributionKey) });
}
