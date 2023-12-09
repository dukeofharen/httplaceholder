using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class StubContext(
    IEnumerable<IStubSource> stubSources,
    IOptionsMonitor<SettingsModel> options,
    IRequestNotify requestNotify,
    IStubRequestContext stubRequestContext,
    IStubNotify stubNotify,
    IScenarioNotify scenarioNotify,
    ICacheService cacheService)
    : IStubContext, ISingletonService
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _scenarioLocks = new();

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GetStubsAsync(CancellationToken cancellationToken) =>
        await GetStubsAsync(false, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>>
        GetStubsFromReadOnlySourcesAsync(CancellationToken cancellationToken) =>
        await GetStubsAsync(true, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GetStubsAsync(string tenant, CancellationToken cancellationToken) =>
        (await GetStubsAsync(false, cancellationToken))
        .Where(s => string.Equals(s.Stub.Tenant, tenant, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken)
    {
        var result = new List<FullStubOverviewModel>();
        foreach (var source in stubSources)
        {
            var stubSourceIsReadOnly = source is not IWritableStubSource;
            var fullStubModels =
                (await source.GetStubsOverviewAsync(stubRequestContext.DistributionKey, cancellationToken))
                .Select(s => new FullStubOverviewModel
                {
                    Stub = s, Metadata = new StubMetadataModel {ReadOnly = stubSourceIsReadOnly}
                });
            result.AddRange(fullStubModels);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<FullStubModel> AddStubAsync(StubModel stub, CancellationToken cancellationToken)
    {
        // Check that a stub with the new ID isn't already added to a readonly stub source.
        var stubs = await GetStubsAsync(true, cancellationToken);
        if (stubs.Any(s => string.Equals(stub.Id, s.Stub.Id, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException($"Stub with ID '{stub.Id}'.");
        }

        var source = GetWritableStubSource();
        var distKey = stubRequestContext.DistributionKey;
        await source.AddStubAsync(stub, distKey, cancellationToken);
        var result = new FullStubModel {Stub = stub, Metadata = new StubMetadataModel {ReadOnly = false}};
        var overviewModel = new FullStubOverviewModel
        {
            Stub = new StubOverviewModel {Id = stub.Id, Tenant = stub.Tenant, Enabled = stub.Enabled},
            Metadata = result.Metadata
        };
        await stubNotify.StubAddedAsync(overviewModel, distKey, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken)
    {
        var distKey = stubRequestContext.DistributionKey;
        var result = await GetWritableStubSource()
            .DeleteStubAsync(stubId, distKey, cancellationToken);
        await stubNotify.StubDeletedAsync(stubId, distKey, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    public async Task DeleteAllStubsAsync(string tenant, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        var distKey = stubRequestContext.DistributionKey;
        var stubs = (await source.GetStubsAsync(distKey, cancellationToken))
            .Where(s => string.Equals(s.Tenant, tenant, StringComparison.OrdinalIgnoreCase));
        foreach (var stub in stubs)
        {
            await source.DeleteStubAsync(stub.Id, distKey, cancellationToken);
            await stubNotify.StubDeletedAsync(stub.Id, distKey, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAllStubsAsync(CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        var distKey = stubRequestContext.DistributionKey;
        var stubs = await source.GetStubsAsync(distKey, cancellationToken);
        foreach (var stub in stubs)
        {
            await source.DeleteStubAsync(stub.Id, distKey, cancellationToken);
            await stubNotify.StubDeletedAsync(stub.Id, distKey, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task UpdateAllStubs(string tenant, IEnumerable<StubModel> stubs, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        var stubArray = stubs as StubModel[] ?? stubs.ToArray();
        var stubIds = stubArray
            .Select(s => s.Id)
            .Distinct();
        var distKey = stubRequestContext.DistributionKey;
        var existingStubs = (await source.GetStubsAsync(distKey, cancellationToken))
            .Where(s => stubIds.Any(sid => string.Equals(sid, s.Id, StringComparison.OrdinalIgnoreCase)) ||
                        string.Equals(s.Tenant, tenant, StringComparison.OrdinalIgnoreCase));

        // First, delete the existing stubs.
        foreach (var stub in existingStubs)
        {
            await source.DeleteStubAsync(stub.Id, distKey, cancellationToken);
            await stubNotify.StubDeletedAsync(stub.Id, distKey, cancellationToken);
        }

        // Make sure the new selection of stubs all have the new tenant and add them to the stub source.
        foreach (var stub in stubArray)
        {
            stub.Tenant = tenant;
            await source.AddStubAsync(stub, distKey, cancellationToken);
            var overviewModel = new FullStubOverviewModel
            {
                Stub = new StubOverviewModel {Id = stub.Id, Tenant = stub.Tenant, Enabled = stub.Enabled},
                Metadata = new StubMetadataModel {ReadOnly = false}
            };
            await stubNotify.StubAddedAsync(overviewModel, distKey, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<FullStubModel> GetStubAsync(string stubId, CancellationToken cancellationToken)
    {
        FullStubModel result = null;
        foreach (var source in stubSources)
        {
            var stub = await source.GetStubAsync(stubId, stubRequestContext.DistributionKey, cancellationToken);
            if (stub == null)
            {
                continue;
            }

            result = new FullStubModel
            {
                Stub = stub, Metadata = new StubMetadataModel {ReadOnly = source is not IWritableStubSource}
            };
            break;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();

        var settings = options.CurrentValue;
        if (settings.Storage?.CleanOldRequestsInBackgroundJob == false)
        {
            // Clean up old requests here.
            await source.CleanOldRequestResultsAsync(cancellationToken);
        }

        var distKey = stubRequestContext.DistributionKey;
        var stub = !string.IsNullOrWhiteSpace(requestResult.ExecutingStubId)
            ? await GetStubAsync(requestResult.ExecutingStubId, cancellationToken)
            : null;
        requestResult.StubTenant = stub?.Stub?.Tenant;
        await source.AddRequestResultAsync(requestResult, settings.Storage?.StoreResponses == true ? response : null,
            distKey, cancellationToken);
        await requestNotify.NewRequestReceivedAsync(requestResult, distKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(
        PagingModel pagingModel,
        CancellationToken cancellationToken) =>
        await GetWritableStubSource()
            .GetRequestResultsAsync(pagingModel, stubRequestContext.DistributionKey, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(
        PagingModel pagingModel,
        CancellationToken cancellationToken) =>
        await GetWritableStubSource()
            .GetRequestResultsOverviewAsync(pagingModel, stubRequestContext.DistributionKey, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId,
        CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        var results = await source.GetRequestResultsAsync(null, stubRequestContext.DistributionKey, cancellationToken);
        results = results
            .Where(r => r.ExecutingStubId == stubId)
            .OrderByDescending(s => s.RequestBeginTime);
        return results;
    }

    /// <inheritdoc />
    public async Task<RequestResultModel> GetRequestResultAsync(string correlationId,
        CancellationToken cancellationToken) =>
        await GetWritableStubSource()
            .GetRequestAsync(correlationId, stubRequestContext.DistributionKey, cancellationToken);

    /// <inheritdoc />
    public async Task<ResponseModel> GetResponseAsync(string correlationId, CancellationToken cancellationToken) =>
        await GetWritableStubSource()
            .GetResponseAsync(correlationId, stubRequestContext.DistributionKey, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken) =>
        await GetWritableStubSource()
            .DeleteAllRequestResultsAsync(stubRequestContext.DistributionKey, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken) =>
        await GetWritableStubSource()
            .DeleteRequestAsync(correlationId, stubRequestContext.DistributionKey, cancellationToken);

    /// <inheritdoc />
    public async Task CleanOldRequestResultsAsync(CancellationToken cancellationToken) =>
        await GetWritableStubSource()
            .CleanOldRequestResultsAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetTenantNamesAsync(CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken))
        .Select(s => s.Stub.Tenant)
        .Where(n => !string.IsNullOrWhiteSpace(n))
        .OrderBy(n => n)
        .Distinct();

    /// <inheritdoc />
    public async Task PrepareAsync(CancellationToken cancellationToken) =>
        await Task.WhenAll(stubSources.Select(s => s.PrepareStubSourceAsync(cancellationToken)));

    /// <inheritdoc />
    public async Task IncreaseHitCountAsync(string scenario, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return;
        }

        var key = stubRequestContext.DistributionKey ?? string.Empty;
        await ExecuteLockedScenarioAction(scenario, key, cancellationToken, async () =>
        {
            var stubSource = GetWritableStubSource();
            var scenarioStateModel = await GetOrAddScenarioState(scenario, key, cancellationToken);
            scenarioStateModel.HitCount++;
            await stubSource.UpdateScenarioAsync(scenario, scenarioStateModel, key, cancellationToken);
            cacheService.SetScopedItem(CachingKeys.ScenarioState, scenarioStateModel.Copy());

            await scenarioNotify.ScenarioSetAsync(scenarioStateModel, key, cancellationToken);
            return scenarioStateModel;
        });
    }

    /// <inheritdoc />
    public async Task<int?> GetHitCountAsync(string scenario, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return null;
        }

        var key = stubRequestContext.DistributionKey ?? string.Empty;
        var result = await ExecuteLockedScenarioAction(scenario, key, cancellationToken,
            async () => await GetOrAddScenarioState(scenario, key, cancellationToken));
        return result.HitCount;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScenarioStateModel>> GetAllScenariosAsync(CancellationToken cancellationToken)
    {
        var key = stubRequestContext.DistributionKey ?? string.Empty;
        var stubSource = GetWritableStubSource();
        return await stubSource.GetAllScenariosAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ScenarioStateModel> GetScenarioAsync(string scenario, CancellationToken cancellationToken)
    {
        var key = stubRequestContext.DistributionKey ?? string.Empty;
        var stubSource = GetWritableStubSource();
        return await stubSource.GetScenarioAsync(scenario, key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetScenarioAsync(string scenario, ScenarioStateModel scenarioState,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(scenario) || scenarioState == null)
        {
            return;
        }

        var key = stubRequestContext.DistributionKey ?? string.Empty;
        await ExecuteLockedScenarioAction(scenario, key, cancellationToken, async () =>
        {
            var stubSource = GetWritableStubSource();
            var existingScenario = await stubSource.GetScenarioAsync(scenario, key, cancellationToken);
            if (existingScenario == null)
            {
                if (string.IsNullOrWhiteSpace(scenarioState.State))
                {
                    scenarioState.State = Constants.DefaultScenarioState;
                }

                await stubSource.AddScenarioAsync(scenario, scenarioState, key, cancellationToken);
                cacheService.SetScopedItem(CachingKeys.ScenarioState, scenarioState.Copy());
            }
            else
            {
                if (scenarioState.HitCount == -1)
                {
                    scenarioState.HitCount = existingScenario.HitCount;
                }

                if (string.IsNullOrWhiteSpace(scenarioState.State))
                {
                    scenarioState.State = existingScenario.State;
                }

                await stubSource.UpdateScenarioAsync(scenario, scenarioState, key, cancellationToken);
                cacheService.SetScopedItem(CachingKeys.ScenarioState, scenarioState.Copy());
            }

            return scenarioState;
        });

        await scenarioNotify.ScenarioSetAsync(scenarioState, key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteScenarioAsync(string scenario, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return false;
        }

        var result = false;
        var stubSource = GetWritableStubSource();
        var key = stubRequestContext.DistributionKey ?? string.Empty;
        await ExecuteLockedScenarioAction(scenario, key, cancellationToken, async () =>
        {
            result = await stubSource.DeleteScenarioAsync(scenario, key, cancellationToken);
            cacheService.DeleteScopedItem(CachingKeys.ScenarioState);
            return null;
        });

        await scenarioNotify.ScenarioDeletedAsync(scenario, key, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    public async Task DeleteAllScenariosAsync(CancellationToken cancellationToken)
    {
        var stubSource = GetWritableStubSource();
        var key = stubRequestContext.DistributionKey ?? string.Empty;
        await stubSource.DeleteAllScenariosAsync(key, cancellationToken);
        await scenarioNotify.AllScenariosDeletedAsync(key, cancellationToken);
    }

    private async Task<ScenarioStateModel> ExecuteLockedScenarioAction(string scenario, string distributionKey,
        CancellationToken cancellationToken,
        Func<Task<ScenarioStateModel>> func)
    {
        var semaphore = _scenarioLocks.GetOrAdd(distributionKey, k => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await func();
        }
        finally
        {
            semaphore.Release();
        }
    }

    internal async Task<ScenarioStateModel> GetOrAddScenarioState(
        string scenario,
        string distributionKey,
        CancellationToken cancellationToken)
    {
        var stubSource = GetWritableStubSource();
        var scenarioModel = await stubSource.GetScenarioAsync(scenario, distributionKey, cancellationToken);
        if (scenarioModel == null)
        {
            scenarioModel = await stubSource.AddScenarioAsync(scenario, new ScenarioStateModel(scenario),
                distributionKey,
                cancellationToken);
            cacheService.SetScopedItem(CachingKeys.ScenarioState, scenarioModel.Copy());
            await scenarioNotify.ScenarioSetAsync(scenarioModel, distributionKey, cancellationToken);
        }

        return scenarioModel;
    }

    private IWritableStubSource GetWritableStubSource() =>
        (IWritableStubSource)stubSources.Single(s => s is IWritableStubSource);

    private IEnumerable<IStubSource> GetReadOnlyStubSources() =>
        stubSources.Where(s => s is not IWritableStubSource);

    private async Task<IEnumerable<FullStubModel>> GetStubsAsync(bool readOnly, CancellationToken cancellationToken)
    {
        var result = new List<FullStubModel>();
        foreach (var source in readOnly ? GetReadOnlyStubSources() : stubSources)
        {
            var stubSourceIsReadOnly = source is not IWritableStubSource;
            var stubs = await source.GetStubsAsync(stubRequestContext.DistributionKey, cancellationToken);
            var fullStubModels = stubs.Select(s => new FullStubModel
            {
                Stub = s, Metadata = new StubMetadataModel {ReadOnly = stubSourceIsReadOnly}
            });
            result.AddRange(fullStubModels);
        }

        return result;
    }
}
