using HttPlaceholder.DataLogic.Implementations;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using HttPlaceholder.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.DataLogic
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddDataLogic(this IServiceCollection services)
        {
            services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();
            return services;
        }

        public static IServiceCollection AddStubSources(this IServiceCollection services)
        {
            // The YAML stub source should always be registered.
            services.AddSingleton<IStubSource, YamlFileStubSource>();

            var configurationService = services.GetService<IConfigurationService>();
            var config = configurationService.GetConfiguration();
            if (config.TryGetValue(Constants.ConfigKeys.FileStorageLocation, out string fileStoragePath) && !string.IsNullOrWhiteSpace(fileStoragePath))
            {
                // If "fileStorageLocation" is set, it means HttPlaceholder should read and write files to a specific location.
                services.AddSingleton<IStubSource, FileSystemStubSource>();
            }
            else
            {
                // If no suitable configuration is found, store all stubs in memory by default.
                services.AddSingleton<IStubSource, InMemoryStubSource>();
            }

            return services;
        }
    }
}