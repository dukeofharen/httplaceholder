using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Mappings;

namespace HttPlaceholder.Application.Infrastructure.AutoMapper
{
    public sealed class Map
    {
        public Type Source { get; set; }

        public Type Destination { get; set; }
    }

    public static class MapperProfileHelper
    {
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

        public static IList<Map> LoadStandardMappings(Assembly rootAssembly)
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

        public static IList<IHaveCustomMapping> LoadCustomMappings(Assembly rootAssembly)
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
}
