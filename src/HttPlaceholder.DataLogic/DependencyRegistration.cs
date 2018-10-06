using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.DataLogic.Implementations;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using System.Collections.Generic;

namespace HttPlaceholder.DataLogic
{
   public static class DependencyRegistration
   {
      public static IServiceCollection AddDataLogic(this IServiceCollection services)
      {
         services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();
         return services;
      }
      
      public static IServiceCollection AddStubSources(this IServiceCollection services, IDictionary<string, string> config)
      {
         services.AddSingleton<IStubSource, InMemoryStubSource>();
         services.AddSingleton<IStubSource, YamlFileStubSource>();
         return services;
      }
   }
}
