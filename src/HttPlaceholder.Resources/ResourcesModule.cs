using HttPlaceholder.Application.Interfaces.Resources;
using HttPlaceholder.Resources.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Resources;

/// <summary>
/// A class for registering all classes in the Resources project.
/// </summary>
public static class ResourcesModule
{
    /// <summary>
    /// Adds the resources module classes.
    /// </summary>
    public static IServiceCollection AddResourcesModule(this IServiceCollection services) =>
        services.AddSingleton<IResourcesService, ResourcesService>();
}
