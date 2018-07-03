using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.DataLogic.Implementations;
using HttPlaceholder.DataLogic.Implementations.StubSources;

namespace HttPlaceholder.DataLogic
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();

         // Stub sources
         services.AddSingleton<IStubSource, InMemoryStubSource>();
         services.AddSingleton<IStubSource, YamlFileStubSource>();
      }
   }
}
