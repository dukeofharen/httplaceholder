using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Mappings;

namespace HttPlaceholder.Application.Infrastructure.AutoMapper;

/// <summary>
///     A helper class for initializing the AutoMapper profile.
/// </summary>
public static class MapperProfileHelper
{
    /// <summary>
    ///     A method for searching for mappings in the application and registering them to the AutoMapper profile.
    /// </summary>
    /// <param name="profile">The AutoMapper profile.</param>
    /// <param name="assembly">The assembly to search for types in.</param>
    public static void InitializeProfile(this Profile profile, Assembly assembly)
    {
        var types = assembly.GetExportedTypes();
        foreach (var map in LoadStandardMappings(types))
        {
            profile.CreateMap(map.Source, map.Destination).ReverseMap();
        }

        foreach (var map in LoadCustomMappings(types))
        {
            map.CreateMappings(profile);
        }
    }

    private static IEnumerable<Mapping> LoadStandardMappings(Type[] types)
    {
        var from = GetMappingTypes(types, typeof(IMapFrom<>));
        var to = GetMappingTypes(types, typeof(IMapTo<>));
        return from
            .Select(t => new Mapping(GetGenericType(t), t))
            .Concat(to.Select(t => new Mapping(t, GetGenericType(t))));
    }

    private static IEnumerable<Type> GetMappingTypes(IEnumerable<Type> types, Type interfaceType) =>
        from type in types
        from instance in type.GetInterfaces()
        where
            instance.IsGenericType && instance.GetGenericTypeDefinition() == interfaceType &&
            !type.IsAbstract &&
            !type.IsInterface
        select type;

    private static IEnumerable<IHaveCustomMapping> LoadCustomMappings(IEnumerable<Type> types) =>
        from type in types
        from instance in type.GetInterfaces()
        where
            typeof(IHaveCustomMapping).IsAssignableFrom(type) &&
            !type.IsAbstract &&
            !type.IsInterface
        select (IHaveCustomMapping)Activator.CreateInstance(type);

    private static Type GetGenericType(Type type) => type.GetInterfaces().First().GetGenericArguments().First();

    private sealed class Mapping(Type source, Type destination)
    {
        public Type Source { get; } = source;

        public Type Destination { get; } = destination;
    }
}
