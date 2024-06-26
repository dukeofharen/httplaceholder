﻿using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class HeadersResponseWriterFacts
{
    private readonly HeadersResponseWriter _writer = new();

    [TestMethod]
    public async Task HeadersResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { Headers = null } };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual(0, response.Headers.Count);
    }

    [TestMethod]
    public async Task HeadersResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Headers = new Dictionary<string, string> { { "X-Api-Key", "1223" }, { "X-User-Secret", "abc" } }
            }
        };

        var response = new ResponseModel { Headers = new Dictionary<string, string> { { "X-User-Secret", "3422" } } };

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(2, response.Headers.Count);
        Assert.AreEqual("1223", response.Headers["X-Api-Key"]);
        Assert.AreEqual("abc", response.Headers["X-User-Secret"]);
    }
}
