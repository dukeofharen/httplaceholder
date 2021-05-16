using System;
using System.Text;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// A static class for registering the HttPlaceholder client on your DI container.
    /// </summary>
    public static class HttPlaceholderClientModule
    {
        public static IServiceCollection AddHttPlaceholderClient(
            this IServiceCollection serviceCollection,
            IConfiguration clientConfigSection)
        {
            serviceCollection.Configure<HttPlaceholderClientConfiguration>(clientConfigSection);
            var config = clientConfigSection.Get<HttPlaceholderClientConfiguration>();
            config.Validate();
            return serviceCollection.RegisterHttpClient(config);
        }

        public static IServiceCollection AddHttPlaceholderClient(
            this IServiceCollection serviceCollection,
            Action<HttPlaceholderClientConfiguration> configAction = null)
        {
            var config = new HttPlaceholderClientConfiguration();
            configAction?.Invoke(config);
            config.Validate();
            return serviceCollection.RegisterHttpClient(config);
        }

        private static IServiceCollection RegisterHttpClient(
            this IServiceCollection serviceCollection,
            HttPlaceholderClientConfiguration config)
        {
            serviceCollection.AddHttpClient<IHttPlaceholderClient, HttPlaceholderClient>(client => client.ApplyConfiguration(config));
            return serviceCollection;
        }
    }
}
