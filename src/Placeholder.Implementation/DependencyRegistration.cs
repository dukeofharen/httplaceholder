using Microsoft.Extensions.DependencyInjection;
using Placeholder.Implementation.Implementations;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Services;
using Placeholder.Implementation.Services.Implementations;

namespace Placeholder.Implementation
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         services.AddTransient<IStubRequestExecutor, StubRequestExecutor>();

         // Services
         services.AddTransient<IHttpContextService, HttpContextService>();

         // Condition checkers
         services.AddTransient<IConditionChecker, MethodConditionChecker>();
         services.AddTransient<IConditionChecker, PathConditionChecker>();
      }
   }
}
