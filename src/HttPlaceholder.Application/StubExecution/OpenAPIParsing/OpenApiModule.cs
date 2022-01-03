using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing;

public static class OpenApiModule
{
    public static IServiceCollection AddOpenApiModule(this IServiceCollection services) =>
        services
            .AddSingleton<IOpenApiParser, OpenApiParser>()
            .AddSingleton<IOpenApiFakeDataGenerator, OpenApiFakeDataGenerator>()
            .AddSingleton<IOpenApiDataFiller, OpenApiDataFiller>()
            .AddSingleton<IOpenApiToStubConverter, OpenApiToStubConverter>();
}
