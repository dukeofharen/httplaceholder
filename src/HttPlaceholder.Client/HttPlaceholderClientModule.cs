using System;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Client.Implementations;
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
            if (string.IsNullOrWhiteSpace(config.RootUrl))
            {
                throw new ArgumentException(
                    $"No value set for {nameof(config.RootUrl)} in HttPlaceholder configuration.");
            }

            // TODO assert "/" at end of root URL.

            serviceCollection.AddHttpClient<IHttPlaceholderClient, HttPlaceholderClient>(client =>
                client.BaseAddress = new Uri(config.RootUrl));
            return serviceCollection;
        }
    }
}
