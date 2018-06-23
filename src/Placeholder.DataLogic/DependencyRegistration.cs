using Microsoft.Extensions.DependencyInjection;
using Placeholder.DataLogic.Implementations;
using Placeholder.DataLogic.Implementations.StubSources;

namespace Placeholder.DataLogic
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
