using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing;

/// <summary>
/// A class for registering the OpenAPI classes.
/// </summary>
public static class OpenApiModule
{
    /// <summary>
    /// A method for registering the OpenAPI classes.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddOpenApiModule(this IServiceCollection services) =>
        services
            .AddSingleton<IOpenApiParser, OpenApiParser>()
            .AddSingleton<IOpenApiFakeDataGenerator, OpenApiFakeDataGenerator>()
            .AddSingleton<IOpenApiDataFiller, OpenApiDataFiller>()
            .AddSingleton<IOpenApiToStubConverter, OpenApiToStubConverter>();
}
