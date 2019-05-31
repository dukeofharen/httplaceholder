using Ducode.Essentials.Assembly;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Application.StubExecution.ResponseWriting;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution
{
    public static class StubExecutionModule
    {
        public static IServiceCollection AddStubExecutionModule(this IServiceCollection services)
        {
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

            return services;
        }
    }
}
