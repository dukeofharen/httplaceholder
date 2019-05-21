using Ducode.Essentials.Assembly;
using HttPlaceholder.BusinessLogic.Implementations;
using HttPlaceholder.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.BusinessLogic
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddSingleton<IFinalStubDeterminer, FinalStubDeterminer>();
            services.AddSingleton<IStubContainer, StubContainer>();
            services.AddSingleton<IStubRequestExecutor, StubRequestExecutor>();
            services.AddSingleton<IStubResponseGenerator, StubResponseGenerator>();
            services.AddSingleton<IVariableParser, VariableParser>();

            // Condition checkers
            var conditionCheckerTypes = AssemblyUtilities.GetImplementations<IConditionChecker>("HttPlaceholder");
            foreach (var type in conditionCheckerTypes)
            {
                services.AddTransient(typeof(IConditionChecker), type);
            }

            // Response writers
            var responseWriterTypes = AssemblyUtilities.GetImplementations<IResponseWriter>("HttPlaceholder");
            foreach (var type in responseWriterTypes)
            {
                services.AddTransient(typeof(IResponseWriter), type);
            }

            // Variable handlers
            var variableHandlers = AssemblyUtilities.GetImplementations<IVariableHandler>("HttPlaceholder");
            foreach (var type in variableHandlers)
            {
                services.AddTransient(typeof(IVariableHandler), type);
            }

            return services;
        }
    }
}