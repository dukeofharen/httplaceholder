using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.Implementation.Implementations;
using HttPlaceholder.Implementation.Implementations.ConditionCheckers;
using HttPlaceholder.Implementation.Implementations.ResponseWriters;

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
         services.AddTransient<IConditionChecker, BasicAuthenticationConditionChecker>();
         services.AddTransient<IConditionChecker, BodyConditionChecker>();
         services.AddTransient<IConditionChecker, FullPathConditionChecker>();
         services.AddTransient<IConditionChecker, HeaderConditionChecker>();
         services.AddTransient<IConditionChecker, JsonPathConditionChecker>();
         services.AddTransient<IConditionChecker, MethodConditionChecker>();
         services.AddTransient<IConditionChecker, PathConditionChecker>();
         services.AddTransient<IConditionChecker, QueryStringConditionChecker>();
         services.AddTransient<IConditionChecker, XPathConditionChecker>();

         // Response writers
         services.AddTransient<IResponseWriter, Base64ResponseWriter>();
         services.AddTransient<IResponseWriter, ExtraDurationResponseWriter>();
         services.AddTransient<IResponseWriter, FileResponseWriter>();
         services.AddTransient<IResponseWriter, HeadersResponseWriter>();
         services.AddTransient<IResponseWriter, StatusCodeResponseWriter>();
         services.AddTransient<IResponseWriter, TextResponseWriter>();
      }
   }
}
