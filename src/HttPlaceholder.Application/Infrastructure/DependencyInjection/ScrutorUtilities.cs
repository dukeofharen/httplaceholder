using Scrutor;

namespace HttPlaceholder.Application.Infrastructure.DependencyInjection;

/// <summary>
///     A static class used for working with Scrutor.
/// </summary>
public static class ScrutorUtilities
{
    /// <summary>
    ///     A method for registering all dependencies in an assembly.
    /// </summary>
    /// <param name="selector">The implementation type selector.</param>
    public static IImplementationTypeSelector RegisterDependencies(this IImplementationTypeSelector selector) =>
        selector.RegisterSingletons().RegisterTransients();

    private static IImplementationTypeSelector RegisterSingletons(this IImplementationTypeSelector selector) =>
        selector.AddClasses<ISingletonService>().WithSingletonLifetime();

    private static IImplementationTypeSelector RegisterTransients(this IImplementationTypeSelector selector) =>
        selector.AddClasses<ITransientService>().WithTransientLifetime();

    private static ILifetimeSelector AddClasses<TInterfaceType>(this IImplementationTypeSelector selector) =>
        selector
            .AddClasses(c => c.AssignableTo<TInterfaceType>(), false)
            .AsImplementedInterfaces(t => t != typeof(TInterfaceType));
}
