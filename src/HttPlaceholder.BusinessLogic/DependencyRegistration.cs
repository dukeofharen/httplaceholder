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
