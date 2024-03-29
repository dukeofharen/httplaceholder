﻿using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class DeleteStubFacts : BaseClientTest
{
    [TestMethod]
    public async Task DeleteStubAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string stubId = "fallback";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Delete, $"{BaseUrl}ph-api/stubs/{stubId}")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.DeleteStubAsync(stubId));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task DeleteStubAsync_ShouldDeleteStub()
    {
        // Arrange
        const string stubId = "fallback";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Delete, $"{BaseUrl}ph-api/stubs/{stubId}")
            .Respond(HttpStatusCode.NoContent)));

        // Act / Assert
        await client.DeleteStubAsync(stubId);
    }
}
