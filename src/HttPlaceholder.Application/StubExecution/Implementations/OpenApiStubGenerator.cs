using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class OpenApiStubGenerator : IOpenApiStubGenerator
{
    private readonly IStubContext _stubContext;
    private readonly IOpenApiParser _openApiParser;
    private readonly IOpenApiToStubConverter _openApiToStubConverter;

    public OpenApiStubGenerator(
        IStubContext stubContext,
        IOpenApiParser openApiParser,
        IOpenApiToStubConverter openApiToStubConverter)
    {
        _stubContext = stubContext;
        _openApiParser = openApiParser;
        _openApiToStubConverter = openApiToStubConverter;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateOpenApiStubs(string input, bool doNotCreateStub)
    {
        try
        {
            var stubs = new List<FullStubModel>();
            var openApiResult = _openApiParser.ParseOpenApiDefinition(input);
            foreach (var line in openApiResult.Lines)
            {
                stubs.Add(await CreateStub(doNotCreateStub, openApiResult.ServerUrl, line));
            }

            return stubs;
        }
        catch (Exception ex)
        {
            throw new ValidationException($"Exception occurred while trying to parse OpenAPI definition: {ex.Message}");
        }
    }

    private async Task<FullStubModel> CreateStub(bool doNotCreateStub, string serverUrl, OpenApiLine line)
    {
        var stub = await _openApiToStubConverter.ConvertToStubAsync(serverUrl, line);
        if (doNotCreateStub)
        {
            return new FullStubModel {Stub = stub, Metadata = new StubMetadataModel()};
        }

        await _stubContext.DeleteStubAsync(stub.Id);
        return await _stubContext.AddStubAsync(stub);
    }
}
