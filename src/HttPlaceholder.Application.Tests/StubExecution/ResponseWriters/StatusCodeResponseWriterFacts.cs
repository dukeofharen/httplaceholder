﻿using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class StatusCodeResponseWriterFacts
{
    private readonly StatusCodeResponseWriter _writer = new();

    [TestMethod]
    public async Task StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { StatusCode = null } };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(200, response.StatusCode);
    }

    [TestMethod]
    public async Task StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { StatusCode = 409 } };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(409, response.StatusCode);
    }

    [TestMethod]
    public async Task
        StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow_NoStatusCodeSetInStub_StatusCodeAlreadySetOnResponse_ShouldNotBeOverwritten()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { StatusCode = null } };

        var response = new ResponseModel { StatusCode = 409 };

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual(409, response.StatusCode);
    }
}
