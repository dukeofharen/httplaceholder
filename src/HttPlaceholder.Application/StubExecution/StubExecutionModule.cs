using Ducode.Essentials.Assembly;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseWriting;
using HttPlaceholder.Application.StubExecution.VariableHandling;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution
{
    public static class StubExecutionModule
    {
        public static IServiceCollection AddStubExecutionModule(this IServiceCollection services)
        {
            services.AddSingleton<IFinalStubDeterminer, FinalStubDeterminer>();
            services.AddSingleton<IStubContainer, StubContainer>();
            services.AddSingleton<IStubRequestExecutor, StubRequestExecutor>();
            services.AddSingleton<IStubResponseGenerator, StubResponseGenerator>();
            services.AddSingleton<IVariableParser, VariableParser>();

            string filter = "HttPlaceholder";

            // Condition checkers
            foreach (var type in AssemblyHelper.GetImplementations<IConditionChecker>(filter))
            {
                services.AddTransient(typeof(IConditionChecker), type);
            }

            // Response writers
            foreach (var type in AssemblyHelper.GetImplementations<IResponseWriter>(filter))
            {
                services.AddTransient(typeof(IResponseWriter), type);
            }

            // Variable handlers
            foreach (var type in AssemblyHelper.GetImplementations<IVariableHandler>(filter))
            {
                services.AddTransient(typeof(IVariableHandler), type);
            }

            return services;
        }
    }
}
