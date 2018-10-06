using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.BusinessLogic.Implementations;
using HttPlaceholder.Utilities;

namespace HttPlaceholder.BusinessLogic
{
   public static class DependencyRegistration
   {
      public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
      {
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

         return services;
      }
   }
}
