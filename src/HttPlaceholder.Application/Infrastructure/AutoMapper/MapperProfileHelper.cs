using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Mappings;

namespace HttPlaceholder.Application.Infrastructure.AutoMapper;

/// <summary>
/// A model for storing the source and destination of an AutoMapper mapping.
/// </summary>
public sealed class Map
{
    /// <summary>
    /// Gets or sets the source type.
    /// </summary>
    public Type Source { get; set; }

    /// <summary>
    /// Gets or sets the destination type.
    /// </summary>
    public Type Destination { get; set; }
}

/// <summary>
/// A helper class for initializing the AutoMapper profile.
/// </summary>
public static class MapperProfileHelper
{
    /// <summary>
    /// A method for searching for mappings in the application and registering them to the AutoMapper profile.
    /// </summary>
    /// <param name="profile">The AutoMapper profile.</param>
    /// <param name="assembly">The assembly to search for types in.</param>
    public static void InitializeProfile(this Profile profile, Assembly assembly)
    {
        foreach (var map in LoadStandardMappings(assembly))
        {
            profile.CreateMap(map.Source, map.Destination).ReverseMap();
        }

        foreach (var map in LoadCustomMappings(assembly))
        {
            map.CreateMappings(profile);
        }
    }

    private static IEnumerable<Map> LoadStandardMappings(Assembly rootAssembly)
    {
        var types = rootAssembly.GetExportedTypes();
        var from = from type in types
            from instance in type.GetInterfaces()
            where
                instance.IsGenericType && instance.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                !type.IsAbstract &&
                !type.IsInterface
            select new Map
            {
                Source = type.GetInterfaces().First().GetGenericArguments().First(),
                Destination = type
            };
        var to = from type in types
            from instance in type.GetInterfaces()
            where
                instance.IsGenericType && instance.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                !type.IsAbstract &&
                !type.IsInterface
            select new Map
            {
                Source = type,
                Destination = type.GetInterfaces().First().GetGenericArguments().First()
            };
        return from.Concat(to).ToList();
    }

    private static IEnumerable<IHaveCustomMapping> LoadCustomMappings(Assembly rootAssembly)
    {
        var types = rootAssembly.GetExportedTypes();
        return (
            from type in types
            from instance in type.GetInterfaces()
            where
                typeof(IHaveCustomMapping).IsAssignableFrom(type) &&
                !type.IsAbstract &&
                !type.IsInterface
            select (IHaveCustomMapping)Activator.CreateInstance(type)).ToList();
    }
}
