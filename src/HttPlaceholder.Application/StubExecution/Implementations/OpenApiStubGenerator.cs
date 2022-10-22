using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class OpenApiStubGenerator : IOpenApiStubGenerator, ISingletonService
{
    private readonly ILogger<OpenApiStubGenerator> _logger;
    private readonly IOpenApiParser _openApiParser;
    private readonly IOpenApiToStubConverter _openApiToStubConverter;
    private readonly IStubContext _stubContext;

    public OpenApiStubGenerator(
        IStubContext stubContext,
        IOpenApiParser openApiParser,
        IOpenApiToStubConverter openApiToStubConverter,
        ILogger<OpenApiStubGenerator> logger)
    {
        _stubContext = stubContext;
        _openApiParser = openApiParser;
        _openApiToStubConverter = openApiToStubConverter;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateOpenApiStubsAsync(string input, bool doNotCreateStub,
        string tenant, CancellationToken cancellationToken)
    {
        try
        {
            var stubs = new List<FullStubModel>();
            var openApiResult = _openApiParser.ParseOpenApiDefinition(input);
            foreach (var line in openApiResult.Lines)
            {
                stubs.Add(await CreateStub(doNotCreateStub, openApiResult.Server, line, tenant, cancellationToken));
            }

            return stubs;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception occurred while trying to create stubs based on OpenAPI definition.");
            throw new ValidationException($"Exception occurred while trying to parse OpenAPI definition: {ex.Message}");
        }
    }

    private async Task<FullStubModel> CreateStub(bool doNotCreateStub, OpenApiServer server, OpenApiLine line,
        string tenant, CancellationToken cancellationToken)
    {
        var stub = await _openApiToStubConverter.ConvertToStubAsync(server, line, tenant, cancellationToken);
        if (doNotCreateStub)
        {
            return new FullStubModel {Stub = stub, Metadata = new StubMetadataModel()};
        }

        await _stubContext.DeleteStubAsync(stub.Id, cancellationToken);
        return await _stubContext.AddStubAsync(stub, cancellationToken);
    }
}
