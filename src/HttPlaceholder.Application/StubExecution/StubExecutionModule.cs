﻿using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseWriting;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Application.StubExecution.VariableHandling;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution
{
    public static class StubExecutionModule
    {
        public static IServiceCollection AddStubExecutionModule(this IServiceCollection services)
        {
            services.AddSingleton<IFinalStubDeterminer, FinalStubDeterminer>();
            services.AddSingleton<IStubRequestExecutor, StubRequestExecutor>();
            services.AddSingleton<IStubResponseGenerator, StubResponseGenerator>();
            services.AddSingleton<IRequestLoggerFactory, RequestLoggerFactory>();
            services.AddSingleton<IRequestStubGenerator, RequestStubGenerator>();
            services.AddSingleton<IStubModelValidator, StubModelValidator>();
            services.AddSingleton<IVariableParser, VariableParser>();

            const string filter = "HttPlaceholder";

            // Condition checkers
            foreach (var type in AssemblyHelper.GetImplementations<IConditionChecker>(filter))
            {
                services.AddSingleton(typeof(IConditionChecker), type);
            }

            // Response writers
            foreach (var type in AssemblyHelper.GetImplementations<IResponseWriter>(filter))
            {
                services.AddSingleton(typeof(IResponseWriter), type);
            }

            // Variable handlers
            foreach (var type in AssemblyHelper.GetImplementations<IVariableHandler>(filter))
            {
                services.AddSingleton(typeof(IVariableHandler), type);
            }

            // Request stub generation
            foreach (var type in AssemblyHelper.GetImplementations<IRequestStubGenerationHandler>(filter))
            {
                services.AddSingleton(typeof(IRequestStubGenerationHandler), type);
            }

            return services;
        }
    }
}
