using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.Implementation.Implementations;
using HttPlaceholder.Utilities;

namespace HttPlaceholder.Implementation
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         DataLogic.DependencyRegistration.RegisterDependencies(services);

         services.AddSingleton<IStubContainer, StubContainer>();
         services.AddTransient<IStubRequestExecutor, StubRequestExecutor>();
         services.AddTransient<IStubResponseGenerator, StubResponseGenerator>();

         // Condition checkers
         var conditionCheckerTypes = AssemblyHelper.GetImplementations<IConditionChecker>();
         foreach(var type in conditionCheckerTypes)
         {
            services.AddTransient(typeof(IConditionChecker), type);
         }

         // Response writers
         var responseWriterTypes = AssemblyHelper.GetImplementations<IResponseWriter>();
         foreach (var type in responseWriterTypes)
         {
            services.AddTransient(typeof(IResponseWriter), type);
         }
      }
   }
}
