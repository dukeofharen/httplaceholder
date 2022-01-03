﻿using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class OpenApiStubGeneratorFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task GenerateOpenApiStubs_ExceptionOccurred_ShouldThrowValidationException()
    {
        // Arrange
        var openApiParserMock = _mocker.GetMock<IOpenApiParser>();
        var generator = _mocker.CreateInstance<OpenApiStubGenerator>();

        const string input = "openapi input";

        openApiParserMock
            .Setup(m => m.ParseOpenApiDefinition(input))
            .Throws(new Exception("ERROR!"));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ValidationException>(() => generator.GenerateOpenApiStubs(input, true));

        // Assert
        Assert.AreEqual("Validation failed:\nException occurred while trying to parse OpenAPI definition: ERROR!",
            exception.Message);
    }

    [TestMethod]
    public async Task GenerateOpenApiStubs_HappyFlow_DoNotCreateStubs()
    {
        // Arrange
        var openApiParserMock = _mocker.GetMock<IOpenApiParser>();
        var openApiToStubConverterMock = _mocker.GetMock<IOpenApiToStubConverter>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var generator = _mocker.CreateInstance<OpenApiStubGenerator>();

        const string input = "openapi input";

        const string serverUrl = "http://localhost";
        var lines = new[] {new OpenApiLine(), new OpenApiLine()};
        var openApiResult = new OpenApiResult {ServerUrl = serverUrl, Lines = lines};
        openApiParserMock
            .Setup(m => m.ParseOpenApiDefinition(input))
            .Returns(openApiResult);

        var stub1 = new StubModel {Id = "stub1"};
        openApiToStubConverterMock
            .Setup(m => m.ConvertToStubAsync(serverUrl, lines[0]))
            .ReturnsAsync(stub1);

        var stub2 = new StubModel {Id = "stub2"};
        openApiToStubConverterMock
            .Setup(m => m.ConvertToStubAsync(serverUrl, lines[1]))
            .ReturnsAsync(stub2);

        // Act
        var result = (await generator.GenerateOpenApiStubs(input, true)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(stub1, result[0].Stub);
        Assert.AreEqual(stub2, result[1].Stub);

        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>()), Times.Never());
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>()), Times.Never());
    }

    [TestMethod]
    public async Task GenerateOpenApiStubs_HappyFlow_CreateStubs()
    {
        // Arrange
        var openApiParserMock = _mocker.GetMock<IOpenApiParser>();
        var openApiToStubConverterMock = _mocker.GetMock<IOpenApiToStubConverter>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var generator = _mocker.CreateInstance<OpenApiStubGenerator>();

        const string input = "openapi input";

        const string serverUrl = "http://localhost";
        var lines = new[] {new OpenApiLine(), new OpenApiLine()};
        var openApiResult = new OpenApiResult {ServerUrl = serverUrl, Lines = lines};
        openApiParserMock
            .Setup(m => m.ParseOpenApiDefinition(input))
            .Returns(openApiResult);

        var stub1 = new StubModel {Id = "stub1"};
        openApiToStubConverterMock
            .Setup(m => m.ConvertToStubAsync(serverUrl, lines[0]))
            .ReturnsAsync(stub1);
        var addedStub1 = new StubModel {Id = "stub1"};
        stubContextMock
            .Setup(m => m.AddStubAsync(stub1))
            .ReturnsAsync(new FullStubModel{Stub = addedStub1});

        var stub2 = new StubModel {Id = "stub2"};
        openApiToStubConverterMock
            .Setup(m => m.ConvertToStubAsync(serverUrl, lines[1]))
            .ReturnsAsync(stub2);
        var addedStub2 = new StubModel {Id = "stub2"};
        stubContextMock
            .Setup(m => m.AddStubAsync(stub2))
            .ReturnsAsync(new FullStubModel{Stub = addedStub2});

        // Act
        var result = (await generator.GenerateOpenApiStubs(input, false)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(addedStub1, result[0].Stub);
        Assert.AreEqual(addedStub2, result[1].Stub);

        stubContextMock.Verify(m => m.DeleteStubAsync("stub1"));
        stubContextMock.Verify(m => m.AddStubAsync(stub1));
        stubContextMock.Verify(m => m.DeleteStubAsync("stub2"));
        stubContextMock.Verify(m => m.AddStubAsync(stub2));
    }
}