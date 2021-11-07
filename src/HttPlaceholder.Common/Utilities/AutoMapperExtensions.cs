using System;
using AutoMapper;

namespace HttPlaceholder.Common.Utilities
{
    public static class AutoMapperExtensions
    {
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
}
