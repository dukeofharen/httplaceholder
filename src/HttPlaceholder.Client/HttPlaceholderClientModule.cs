using System;
using System.Text;
using HttPlaceholder.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Client
{
    public static class HttPlaceholderClientModule
    {
        public static IServiceCollection AddHttPlaceholderClient(this IServiceCollection services, Action<HttPlaceholderClientSettings> configure = null)
        {
            configure = configure ?? (_ => { });
            services.Configure(configure);
            services.AddHttpClient<IHttPlaceholderClientFactory, HttPlaceholderClientFactory>((serviceProvider, httpClient) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<HttPlaceholderClientSettings>>().Value;
                if (!string.IsNullOrWhiteSpace(settings.Username) && !string.IsNullOrWhiteSpace(settings.Password))
                {
                    string basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{settings.Username}:{settings.Password}"));
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {basic}");
                }
            });
            return services;
        }
    }
}
