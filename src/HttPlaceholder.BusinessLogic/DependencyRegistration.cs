using Ducode.Essentials.Assembly;
using HttPlaceholder.BusinessLogic.Implementations;
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
            var conditionCheckerTypes = AssemblyHelper.GetImplementations<IConditionChecker>();
            foreach (var type in conditionCheckerTypes)
            {
                services.AddTransient(typeof(IConditionChecker), type);
            }

            // Response writers
            var responseWriterTypes = AssemblyHelper.GetImplementations<IResponseWriter>();
            foreach (var type in responseWriterTypes)
            {
                services.AddTransient(typeof(IResponseWriter), type);
            }

            // Variable handlers
            var variableHandlers = AssemblyHelper.GetImplementations<IVariableHandler>();
            foreach (var type in variableHandlers)
            {
                services.AddTransient(typeof(IVariableHandler), type);
            }

            return services;
        }
    }
}