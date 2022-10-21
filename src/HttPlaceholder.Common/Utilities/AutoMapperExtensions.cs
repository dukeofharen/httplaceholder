using System;
using AutoMapper;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class with AutoMapper helper methods.
/// </summary>
public static class AutoMapperExtensions
{
    /// <summary>
    ///     Maps an object to another object and gives an option to mutate the mapped object right after mapping.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="source">The source object.</param>
    /// <param name="action">The action that is used to mutate the mapped object.</param>
    /// <typeparam name="TDestination">The type the object should be mapped to.</typeparam>
    /// <returns>The mapped object.</returns>
    public static TDestination MapAndSet<TDestination>(
        this IMapper mapper,
        object source,
        Action<TDestination> action)
    {
        var destination = mapper.Map<TDestination>(source);
        action(destination);
        return destination;
    }
}
