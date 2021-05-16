using System;
using System.Text;
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
            ValidateConfiguration(config);
            return serviceCollection.RegisterHttpClient(config);
        }

        public static IServiceCollection AddHttPlaceholderClient(
            this IServiceCollection serviceCollection,
            Action<HttPlaceholderClientConfiguration> configAction = null)
        {
            var config = new HttPlaceholderClientConfiguration();
            configAction?.Invoke(config);
            ValidateConfiguration(config);
            return serviceCollection.RegisterHttpClient(config);
        }

        private static IServiceCollection RegisterHttpClient(
            this IServiceCollection serviceCollection,
            HttPlaceholderClientConfiguration config)
        {
            serviceCollection.AddHttpClient<IHttPlaceholderClient, HttPlaceholderClient>(client =>
            {
                client.BaseAddress = new Uri(config.RootUrl);
                if (!string.IsNullOrWhiteSpace(config.Username) && !string.IsNullOrWhiteSpace(config.Password))
                {
                    var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}"));
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
                }
            });
            return serviceCollection;
        }

        private static void ValidateConfiguration(HttPlaceholderClientConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.RootUrl))
            {
                throw new ArgumentException(
                    $"No value set for {nameof(config.RootUrl)} in HttPlaceholder configuration.");
            }
        }
    }
}
