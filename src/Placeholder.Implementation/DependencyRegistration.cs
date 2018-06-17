using Microsoft.Extensions.DependencyInjection;
using Placeholder.Implementation.Implementations;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Implementations.ResponseWriters;

namespace Placeholder.Implementation
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         DataLogic.DependencyRegistration.RegisterDependencies(services);

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
