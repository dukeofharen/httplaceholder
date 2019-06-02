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
            services.AddHttpClient<IHttPlaceholderClientFactory, HttPlaceholderClientFactory>();
            return services;
        }
    }
}
