using Scrutor;

namespace HttPlaceholder.Application.Infrastructure.DependencyInjection;

/// <summary>
/// A static class used for working with Scrutor.
/// </summary>
public static class ScrutorUtilities
{
    /// <summary>
    /// A method for registering all singleton dependencies.
    /// </summary>
    /// <param name="selector">The implementation type selector.</param>
    public static IImplementationTypeSelector RegisterSingletons(this IImplementationTypeSelector selector) =>
        selector.AddClasses(c => c.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces(t => t != typeof(ISingletonService))
            .WithSingletonLifetime();

    /// <summary>
    /// A method for registering all transient dependencies.
    /// </summary>
    /// <param name="selector">The implementation type selector.</param>
    public static IImplementationTypeSelector RegisterTransients(this IImplementationTypeSelector selector) =>
        selector.AddClasses(c => c.AssignableTo<ITransientService>())
            .AsImplementedInterfaces(t => t != typeof(ITransientService))
            .WithTransientLifetime();

    /// <summary>
    /// A method for registering all dependencies in an assembly.
    /// </summary>
    /// <param name="selector">The implementation type selector.</param>
    public static IImplementationTypeSelector RegisterDependencies(this IImplementationTypeSelector selector) =>
        selector.RegisterSingletons().RegisterTransients();
}
