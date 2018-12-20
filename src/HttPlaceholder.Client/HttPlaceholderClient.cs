using System;
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
                var result = await RootUrl
                .AppendPathSegment($"users/{username}")
                .WithBasicAuth(username, password)
                .GetJsonAsync<UserModel>();
                return result;
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
    }
}
