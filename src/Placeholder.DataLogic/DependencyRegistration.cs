using Microsoft.Extensions.DependencyInjection;
using Placeholder.DataLogic.Implementations;

namespace Placeholder.DataLogic
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         services.AddSingleton<IStubContainer, StubContainer>();
      }
   }
}
