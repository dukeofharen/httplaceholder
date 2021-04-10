﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources
{
    internal class RelationalDbStubSource : IWritableStubSource
    {
        private const string StubJsonType = "json";
        private const string StubYamlType = "yaml";

        private readonly SettingsModel _settings;
        private readonly IQueryStore _queryStore;
        private readonly IDatabaseContextFactory _databaseContextFactory;

        public RelationalDbStubSource(
            IOptions<SettingsModel> options,
            IQueryStore queryStore,
            IDatabaseContextFactory databaseContextFactory)
        {
            _settings = options.Value;
            _queryStore = queryStore;
            _databaseContextFactory = databaseContextFactory;
        }

        public async Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
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
        }

        public async Task AddStubAsync(StubModel stub)
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                var json = JsonConvert.SerializeObject(stub);
                await ctx.ExecuteAsync(_queryStore.AddStubQuery,
                    new {StubId = stub.Id, Stub = json, StubType = StubJsonType});
            }
        }

        public async Task CleanOldRequestResultsAsync()
        {
            var maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                await ctx.ExecuteAsync(_queryStore.CleanOldRequestsQuery, new {Limit = maxLength});
            }
        }

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

        public async Task<RequestResultModel> GetRequestAsync(string correlationId)
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                var result = await ctx.QueryFirstOrDefaultAsync<DbRequestModel>(_queryStore.GetRequestQuery,
                    new {CorrelationId = correlationId});
                if (result == null)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<RequestResultModel>(result.Json);
            }
        }

        public async Task DeleteAllRequestResultsAsync()
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                await ctx.ExecuteAsync(_queryStore.DeleteAllRequestsQuery);
            }
        }

        public async Task<bool> DeleteStubAsync(string stubId)
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                var updated = await ctx.ExecuteAsync(_queryStore.DeleteStubQuery, new {StubId = stubId});
                return updated > 0;
            }
        }

        public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                var result = await ctx.QueryAsync<DbRequestModel>(_queryStore.GetRequestsQuery);
                return result
                    .Select(r => JsonConvert.DeserializeObject<RequestResultModel>(r.Json));
            }
        }

        public async Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                var queryResults = await ctx.QueryAsync<DbStubModel>(_queryStore.GetStubsQuery);
                var result = new List<StubModel>();
                foreach (var queryResult in queryResults)
                {
                    switch (queryResult.StubType)
                    {
                        case StubJsonType:
                            result.Add(JsonConvert.DeserializeObject<StubModel>(queryResult.Stub));
                            break;

                        case StubYamlType:
                            result.Add(YamlUtilities.Parse<StubModel>(queryResult.Stub));
                            break;

                        default:
                            throw new NotImplementedException(
                                $"StubType '{queryResult.StubType}' not supported: stub '{queryResult.StubId}'.");
                    }
                }

                return result;
            }
        }

        public async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync() =>
            (await GetStubsAsync())
            .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
            .ToArray();

        public async Task<StubModel> GetStubAsync(string stubId)
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                var result = await ctx.QueryFirstOrDefaultAsync<DbStubModel>(_queryStore.GetStubQuery,
                    new {StubId = stubId});
                if (result == null)
                {
                    return null;
                }

                switch (result.StubType)
                {
                    case StubJsonType:
                        return JsonConvert.DeserializeObject<StubModel>(result.Stub);
                    case StubYamlType:
                        return YamlUtilities.Parse<StubModel>(result.Stub);
                    default:
                        throw new NotImplementedException(
                            $"StubType '{result.StubType}' not supported: stub '{stubId}'.");
                }
            }
        }

        public async Task PrepareStubSourceAsync()
        {
            using (var ctx = _databaseContextFactory.CreateDatabaseContext())
            {
                await ctx.ExecuteAsync(_queryStore.MigrationsQuery);
            }
        }
    }
}
