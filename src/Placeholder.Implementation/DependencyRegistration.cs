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
         services.AddSingleton<IStubContainer, StubContainer>();
         services.AddTransient<IStubManager, StubManager>();
         services.AddTransient<IStubRequestExecutor, StubRequestExecutor>();

         // Services
         services.AddTransient<IFileService, FileService>();
         services.AddTransient<IHttpContextService, HttpContextService>();
         services.AddTransient<IYamlService, YamlService>();

         // Condition checkers
         services.AddTransient<IConditionChecker, BodyConditionChecker>();
         services.AddTransient<IConditionChecker, HeaderConditionChecker>();
         services.AddTransient<IConditionChecker, MethodConditionChecker>();
         services.AddTransient<IConditionChecker, PathConditionChecker>();
         services.AddTransient<IConditionChecker, QueryStringConditionChecker>();
      }
   }
}
