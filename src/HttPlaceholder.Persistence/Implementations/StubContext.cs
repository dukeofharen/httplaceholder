﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations;

/// <inheritdoc />
internal class StubContext : IStubContext
{
    private readonly IEnumerable<IStubSource> _stubSources;
    private readonly SettingsModel _settings;
    private readonly IRequestNotify _requestNotify;

    public StubContext(IEnumerable<IStubSource> stubSources, IOptions<SettingsModel> options,
        IRequestNotify requestNotify)
    {
        _stubSources = stubSources;
        _requestNotify = requestNotify;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GetStubsAsync(CancellationToken cancellationToken) =>
        await GetStubsAsync(false, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>>
        GetStubsFromReadOnlySourcesAsync(CancellationToken cancellationToken) =>
        await GetStubsAsync(true, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GetStubsAsync(string tenant, CancellationToken cancellationToken)
    {
        var stubs = await GetStubsAsync(false, cancellationToken);
        return stubs
            .Where(s => string.Equals(s.Stub.Tenant, tenant, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken)
    {
        var result = new List<FullStubOverviewModel>();
        foreach (var source in _stubSources)
        {
            var stubSourceIsReadOnly = source is not IWritableStubSource;
            var stubs = await source.GetStubsOverviewAsync(cancellationToken);
            var fullStubModels = stubs.Select(s => new FullStubOverviewModel
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
        await source.AddStubAsync(stub, cancellationToken);
        return new FullStubModel {Stub = stub, Metadata = new StubMetadataModel {ReadOnly = false}};
    }

    /// <inheritdoc />
    public async Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken) =>
        await GetWritableStubSource().DeleteStubAsync(stubId, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteAllStubsAsync(string tenant, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        var stubs = (await source.GetStubsAsync(cancellationToken))
            .Where(s => string.Equals(s.Tenant, tenant, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        foreach (var stub in stubs)
        {
            await source.DeleteStubAsync(stub.Id, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAllStubsAsync(CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        var stubs = (await source.GetStubsAsync(cancellationToken)).ToArray();
        foreach (var stub in stubs)
        {
            await source.DeleteStubAsync(stub.Id, cancellationToken);
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
        var existingStubs = (await source.GetStubsAsync(cancellationToken))
            .Where(s => stubIds.Any(sid => string.Equals(sid, s.Id, StringComparison.OrdinalIgnoreCase)) ||
                        string.Equals(s.Tenant, tenant, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        // First, delete the existing stubs.
        foreach (var stub in existingStubs)
        {
            await source.DeleteStubAsync(stub.Id, cancellationToken);
        }

        // Make sure the new selection of stubs all have the new tenant and add them to the stub source.
        foreach (var stub in stubArray)
        {
            stub.Tenant = tenant;
            await source.AddStubAsync(stub, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<FullStubModel> GetStubAsync(string stubId, CancellationToken cancellationToken)
    {
        FullStubModel result = null;
        foreach (var source in _stubSources)
        {
            var stub = await source.GetStubAsync(stubId, cancellationToken);
            if (stub != null)
            {
                result = new FullStubModel
                {
                    Stub = stub, Metadata = new StubMetadataModel {ReadOnly = source is not IWritableStubSource}
                };
                break;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel response, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();

        if (_settings.Storage?.CleanOldRequestsInBackgroundJob == false)
        {
            // Clean up old requests here.
            await source.CleanOldRequestResultsAsync(cancellationToken);
        }

        var stub = !string.IsNullOrWhiteSpace(requestResult.ExecutingStubId)
            ? await GetStubAsync(requestResult.ExecutingStubId, cancellationToken)
            : null;
        requestResult.StubTenant = stub?.Stub?.Tenant;
        await source.AddRequestResultAsync(requestResult, _settings.Storage?.StoreResponses == true ? response : null, cancellationToken);
        await _requestNotify.NewRequestReceivedAsync(requestResult, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(CancellationToken cancellationToken) =>
        (await GetWritableStubSource().GetRequestResultsAsync(cancellationToken))
        .OrderByDescending(s => s.RequestBeginTime);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(CancellationToken cancellationToken) =>
        (await GetWritableStubSource().GetRequestResultsOverviewAsync(cancellationToken))
        .OrderByDescending(s => s.RequestEndTime);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        var results = await source.GetRequestResultsAsync(cancellationToken);
        results = results
            .Where(r => r.ExecutingStubId == stubId)
            .OrderByDescending(s => s.RequestBeginTime);
        return results;
    }

    /// <inheritdoc />
    public async Task<RequestResultModel> GetRequestResultAsync(string correlationId, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        return await source.GetRequestAsync(correlationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ResponseModel> GetResponseAsync(string correlationId, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        return await source.GetResponseAsync(correlationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        await source.DeleteAllRequestResultsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        return await source.DeleteRequestAsync(correlationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task CleanOldRequestResultsAsync(CancellationToken cancellationToken)
    {
        var source = GetWritableStubSource();
        await source.CleanOldRequestResultsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetTenantNamesAsync(CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken))
        .Select(s => s.Stub.Tenant)
        .Where(n => !string.IsNullOrWhiteSpace(n))
        .OrderBy(n => n)
        .Distinct();

    /// <inheritdoc />
    public async Task PrepareAsync(CancellationToken cancellationToken)
    {
        foreach (var source in _stubSources)
        {
            // Call PrepareStubSourceAsync on all stub sources for letting them prepare their own setup (e.g. create tables, folders etc.).
            await source.PrepareStubSourceAsync(cancellationToken);
        }
    }

    private IWritableStubSource GetWritableStubSource() =>
        (IWritableStubSource)_stubSources.Single(s => s is IWritableStubSource);

    private IEnumerable<IStubSource> GetReadOnlyStubSources() =>
        _stubSources.Where(s => s is not IWritableStubSource);

    private async Task<IEnumerable<FullStubModel>> GetStubsAsync(bool readOnly, CancellationToken cancellationToken)
    {
        var result = new List<FullStubModel>();
        foreach (var source in readOnly ? GetReadOnlyStubSources() : _stubSources)
        {
            var stubSourceIsReadOnly = source is not IWritableStubSource;
            var stubs = await source.GetStubsAsync(cancellationToken);
            var fullStubModels = stubs.Select(s => new FullStubModel
            {
                Stub = s, Metadata = new StubMetadataModel {ReadOnly = stubSourceIsReadOnly}
            });
            result.AddRange(fullStubModels);
        }

        return result;
    }
}
