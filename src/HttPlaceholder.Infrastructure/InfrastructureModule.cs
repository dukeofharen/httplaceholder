using HttPlaceholder.Application.Interfaces.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Infrastructure.Configuration;
using HttPlaceholder.Infrastructure.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HttPlaceholder.Infrastructure;

/// <summary>
/// A module for registering all classes in the Infrastructure project to the service collection.
/// </summary>
public static class InfrastructureModule
{
    /// <summary>
    /// Register all classes in the Infrastructure project to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
    {
        services.TryAddSingleton<IAssemblyService, AssemblyService>();
        services.TryAddSingleton<IAsyncService, AsyncService>();
        services.TryAddSingleton<IConfigurationHelper, ConfigurationHelper>();
        services.TryAddSingleton<IDateTime, MachineDateTime>();
        services.TryAddSingleton<IEnvService, EnvService>();
        services.TryAddSingleton<IFileService, FileService>();
        services.TryAddSingleton<IModelValidator, ModelValidator>();
        return services;
    }
}
