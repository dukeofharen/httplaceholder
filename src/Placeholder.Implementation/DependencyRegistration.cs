using Microsoft.Extensions.DependencyInjection;
using Placeholder.Implementation.Implementations;
using Placeholder.Implementation.Implementations.ConditionCheckers;

namespace Placeholder.Implementation
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         services.AddTransient<IStubRequestExecutor, StubRequestExecutor>();

         // Condition checkers
         services.AddTransient<IConditionChecker, PathConditionChecker>();
      }
   }
}
