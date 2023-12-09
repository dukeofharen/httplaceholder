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

internal class OpenApiStubGenerator(
    IStubContext stubContext,
    IOpenApiParser openApiParser,
    IOpenApiToStubConverter openApiToStubConverter,
    ILogger<OpenApiStubGenerator> logger)
    : IOpenApiStubGenerator, ISingletonService
{
    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateStubsAsync(string input, bool doNotCreateStub, string tenant,
        string stubIdPrefix,
        CancellationToken cancellationToken)
    {
        try
        {
            var stubs = new List<FullStubModel>();
            var openApiResult = openApiParser.ParseOpenApiDefinition(input);
            foreach (var line in openApiResult.Lines)
            {
                stubs.Add(await CreateStub(doNotCreateStub, openApiResult.Server, line, tenant, stubIdPrefix,
                    cancellationToken));
            }

            return stubs;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception occurred while trying to create stubs based on OpenAPI definition.");
            throw new ValidationException($"Exception occurred while trying to parse OpenAPI definition: {ex.Message}");
        }
    }

    private async Task<FullStubModel> CreateStub(bool doNotCreateStub, OpenApiServer server, OpenApiLine line,
        string tenant, string stubIdPrefix, CancellationToken cancellationToken)
    {
        var stub = await openApiToStubConverter.ConvertToStubAsync(server, line, tenant, stubIdPrefix,
            cancellationToken);
        if (doNotCreateStub)
        {
            return new FullStubModel {Stub = stub, Metadata = new StubMetadataModel()};
        }

        await stubContext.DeleteStubAsync(stub.Id, cancellationToken);
        return await stubContext.AddStubAsync(stub, cancellationToken);
    }
}
