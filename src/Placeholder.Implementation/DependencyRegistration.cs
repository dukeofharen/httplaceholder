using Microsoft.Extensions.DependencyInjection;
using Placeholder.Implementation.Implementations;

namespace Placeholder.Implementation
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         services.AddTransient<IStubRequestExecutor, StubRequestExecutor>();
      }
   }
}
