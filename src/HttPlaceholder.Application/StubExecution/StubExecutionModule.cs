using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution;

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
        services.AddSingleton<IResponseVariableParser, ResponseVariableParser>();
        services.AddSingleton<IScenarioService, ScenarioService>();
        services.AddSingleton<ICurlStubGenerator, CurlStubGenerator>();
        services.AddSingleton<ICurlToHttpRequestMapper, CurlToHttpRequestMapper>();
        services.AddSingleton<IHttpRequestToConditionsService, HttpRequestToConditionsService>();

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

        // Response variable parsing handlers
        foreach (var type in AssemblyHelper.GetImplementations<IResponseVariableParsingHandler>(filter))
        {
            services.AddSingleton(typeof(IResponseVariableParsingHandler), type);
        }

        // Request to stub conditions handlers
        foreach (var type in AssemblyHelper.GetImplementations<IRequestToStubConditionsHandler>(filter))
        {
            services.AddSingleton(typeof(IRequestToStubConditionsHandler), type);
        }

        return services;
    }
}