using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution;

public static class StubExecutionModule
{
    public static IServiceCollection AddStubExecutionModule(this IServiceCollection services)
    {
        services
            .AddOpenApiModule()
            .AddSingleton<IFinalStubDeterminer, FinalStubDeterminer>()
            .AddSingleton<IStubRequestExecutor, StubRequestExecutor>()
            .AddSingleton<IStubResponseGenerator, StubResponseGenerator>()
            .AddSingleton<IRequestLoggerFactory, RequestLoggerFactory>()
            .AddSingleton<IRequestStubGenerator, RequestStubGenerator>()
            .AddSingleton<IStubModelValidator, StubModelValidator>()
            .AddSingleton<IResponseVariableParser, ResponseVariableParser>()
            .AddSingleton<IScenarioService, ScenarioService>()
            .AddSingleton<ICurlStubGenerator, CurlStubGenerator>()
            .AddSingleton<IHarStubGenerator, HarStubGenerator>()
            .AddSingleton<ICurlToHttpRequestMapper, CurlToHttpRequestMapper>()
            .AddSingleton<IHttpRequestToConditionsService, HttpRequestToConditionsService>()
            .AddSingleton<IHttpResponseToStubResponseService, HttpResponseToStubResponseService>()
            .AddSingleton<IOpenApiStubGenerator, OpenApiStubGenerator>();

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

        // Response to stub response handlers
        foreach (var type in AssemblyHelper.GetImplementations<IResponseToStubResponseHandler>(filter))
        {
            services.AddSingleton(typeof(IResponseToStubResponseHandler), type);
        }

        return services;
    }
}
