using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Models;
using HttPlaceholder.Models;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// A class that is used to make REST calls to the HttPlaceholder REST API.
    /// </summary>
    public class HttPlaceholderClient : IHttPlaceholderClient
    {
        private readonly HttPlaceholderSettingsModel _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttPlaceholderClient"/> class.
        /// </summary>
        /// <param name="settingsProvider">The settings provider.</param>
        public HttPlaceholderClient(IHttPlaceholderSettingsProvider settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttPlaceholderClient"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public HttPlaceholderClient(HttPlaceholderSettingsModel settings)
        {
            _settings = settings;
        }

        private string RootUrl => _settings.RootUrl + (!_settings.RootUrl.EndsWith("/") ? "/" : string.Empty) + "ph-api";

        /// <summary>
        /// Adds a stub.
        /// </summary>
        /// <param name="stub">The stub.</param>
        /// <remarks>
        /// If a stub with the same ID already exists, it will be overwritten.
        /// </remarks>
        public async Task AddStubAsync(StubModel stub)
        {
            await DoRequest(async () =>
            {
                await RootUrl
                .AppendPathSegment("stubs")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .PostJsonAsync(stub);
            });
        }

        /// <summary>
        /// Deletes all requests.
        /// </summary>
        public async Task DeleteAllRequestsAsync()
        {
            await DoRequest(async () =>
            {
                await RootUrl
                .AppendPathSegment("requests")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .DeleteAsync();
            });
        }

        /// <summary>
        /// Delete all stubs in a given tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        public async Task DeleteAllStubsInTenant(string tenant)
        {
            await DoRequest(async () =>
            {
                await RootUrl
                .AppendPathSegment($"tenants/{tenant}/stubs")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .DeleteAsync();
            });
        }

        /// <summary>
        /// Deletes a stub.
        /// </summary>
        /// <param name="stubId">The stub identifier.</param>
        public async  Task DeleteStubAsync(string stubId)
        {
            await DoRequest(async () =>
            {
                await RootUrl
                .AppendPathSegment($"stubs/{stubId}")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .DeleteAsync();
            });
        }

        /// <summary>
        /// Retrieves all requests.
        /// </summary>
        /// <returns>
        /// A list of <see cref="RequestResultModel" />.
        /// </returns>
        public async Task<IEnumerable<RequestResultModel>> GetAllRequestsAsync()
        {
            return await DoRequest(async () =>
            {
                return await RootUrl
                .AppendPathSegment("requests")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .GetJsonAsync<IEnumerable<RequestResultModel>>();
            });
        }

        /// <summary>
        /// Retrieves all request made for a specific stub.
        /// </summary>
        /// <param name="stubId">The stub identifier.</param>
        /// <returns>
        /// A list of <see cref="RequestResultModel" />.
        /// </returns>
        public async Task<IEnumerable<RequestResultModel>> GetAllRequestsByStubIdAsync(string stubId)
        {
            return await DoRequest(async () =>
            {
                return await RootUrl
                .AppendPathSegment($"requests/{stubId}")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .GetJsonAsync<IEnumerable<RequestResultModel>>();
            });
        }

        /// <summary>
        /// Retrieves all stubs.
        /// </summary>
        /// <returns>
        /// A list of <see cref="FullStubModel" />.
        /// </returns>
        public async Task<IEnumerable<FullStubModel>> GetAllStubsAsync()
        {
            return await DoRequest(async () =>
            {
                return await RootUrl
                .AppendPathSegment($"stubs")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .GetJsonAsync<IEnumerable<FullStubModel>>();
            });
        }

        /// <summary>
        /// Retrieve all stubs in a given tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <returns>
        /// A list of <see cref="FullStubModel" />.
        /// </returns>
        public async Task<IEnumerable<FullStubModel>> GetAllStubsInTenant(string tenant)
        {
            return await DoRequest(async () =>
            {
                return await RootUrl
                .AppendPathSegment($"tenants/{tenant}/stubs")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .GetJsonAsync<IEnumerable<FullStubModel>>();
            });
        }

        /// <summary>
        /// Retrieves the HttPlaceholder metadata.
        /// </summary>
        /// <returns>
        /// The <see cref="MetadataModel" />.
        /// </returns>
        public async Task<MetadataModel> GetMetadataAsync()
        {
            return await DoRequest(async () =>
            {
                return await RootUrl
                .AppendPathSegment("metadata")
                .GetJsonAsync<MetadataModel>();
            });
        }

        /// <summary>
        /// Retrieves a stub by ID.
        /// </summary>
        /// <param name="stubId">The stub identifier.</param>
        /// <returns>
        /// The <see cref="StubModel" />.
        /// </returns>
        public async Task<FullStubModel> GetStubAsync(string stubId)
        {
            return await DoRequest(async () =>
            {
                return await RootUrl
                .AppendPathSegment($"stubs/{stubId}")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .GetJsonAsync<FullStubModel>();
            });
        }

        /// <summary>
        /// Checks whether the given credentials belong to a valid user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The <see cref="UserModel" />.
        /// </returns>
        /// <remarks>
        /// If no authentication is configured on HttPlaceholder, this call will always return a successful result.
        /// </remarks>
        public async Task<UserModel> GetUserAsync(string username, string password)
        {
            return await DoRequest(async () =>
            {
                return await RootUrl
                .AppendPathSegment($"users/{username}")
                .WithBasicAuth(username, password)
                .GetJsonAsync<UserModel>();
            });
        }

        /// <summary>
        /// Update all stubs in a given tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="stubs">The stubs.</param>
        public async Task UpdateAllStubsInTenant(string tenant, IEnumerable<StubModel> stubs)
        {
            await DoRequest(async () =>
            {
                await RootUrl
                .AppendPathSegment($"tenants/{tenant}/stubs")
                .WithBasicAuth(_settings.Username, _settings.Password)
                .PutJsonAsync(stubs);
            });
        }

        private async Task<TResult> DoRequest<TResult>(Func<Task<TResult>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception exception)
            {
                throw new HttPlaceholderClientException(exception);
            }
        }

        private async Task DoRequest(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (Exception exception)
            {
                throw new HttPlaceholderClientException(exception);
            }
        }
    }
}
