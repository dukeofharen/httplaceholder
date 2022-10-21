using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.Verification.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using static HttPlaceholder.Client.Verification.Dto.TimesModel;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class VerifyStubCalledFacts : BaseClientTest
{
    [TestMethod]
    public async Task VerifyStubCalledAsync_Passes()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 2);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var result = await client.VerifyStubCalledAsyncInternal(stubId, Exactly(2), DateTime.UtcNow.AddSeconds(-1));

        // Assert
        Assert.IsTrue(result.Passed);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_RequestsAreMadeTooEarly()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.UtcNow.AddMinutes(-2), 2);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var ex = await Assert.ThrowsExceptionAsync<StubVerificationFailedException>(() =>
            client.VerifyStubCalledAsyncInternal(stubId, Exactly(2), DateTime.UtcNow.AddMinutes(-1)));
        var result = ex.VerificationResultModel;

        // Assert
        Assert.IsFalse(result.Passed);
        Assert.AreEqual("Validation failed. Counted '0' requests, but expected '2'.", result.Message);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_MinHits_Passes()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 2);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var result = await client.VerifyStubCalledAsyncInternal(stubId, AtLeast(2), DateTime.UtcNow.AddSeconds(-1));

        // Assert
        Assert.IsTrue(result.Passed);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_MinHits_Fails()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 2);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var ex = await Assert.ThrowsExceptionAsync<StubVerificationFailedException>(() =>
            client.VerifyStubCalledAsyncInternal(stubId, AtLeast(3), DateTime.UtcNow.AddSeconds(-1)));
        var result = ex.VerificationResultModel;

        // Assert
        Assert.IsFalse(result.Passed);
        Assert.AreEqual("Validation failed. Counted '2', but expected at least '3'.", result.Message);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_MaxHits_Passes()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 2);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var result = await client.VerifyStubCalledAsyncInternal(stubId, AtMost(2), DateTime.UtcNow.AddSeconds(-1));

        // Assert
        Assert.IsTrue(result.Passed);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_MaxHits_Fails()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 2);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var ex = await Assert.ThrowsExceptionAsync<StubVerificationFailedException>(() =>
            client.VerifyStubCalledAsyncInternal(stubId, AtMost(1), DateTime.UtcNow.AddSeconds(-1)));
        var result = ex.VerificationResultModel;

        // Assert
        Assert.IsFalse(result.Passed);
        Assert.AreEqual("Validation failed. Counted '2', but expected at most '1'.", result.Message);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_Between_Passes()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 2);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var result = await client.VerifyStubCalledAsyncInternal(stubId, Between(2, 4), DateTime.UtcNow.AddSeconds(-1));

        // Assert
        Assert.IsTrue(result.Passed);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_Between_TooLittle_Fails()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 1);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var ex = await Assert.ThrowsExceptionAsync<StubVerificationFailedException>(() =>
            client.VerifyStubCalledAsyncInternal(stubId, Between(2, 4), DateTime.UtcNow.AddSeconds(-1)));
        var result = ex.VerificationResultModel;

        // Assert
        Assert.IsFalse(result.Passed);
        Assert.AreEqual("Validation failed. Counted '1', but expected at least '2'.", result.Message);
    }

    [TestMethod]
    public async Task VerifyStubCalledAsync_Between_TooMany_Fails()
    {
        // Arrange
        const string stubId = "stub-id";
        var requestResults = CreateResponse(stubId, DateTime.Now, 5);
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", JsonConvert.SerializeObject(requestResults))));

        // Act
        var ex = await Assert.ThrowsExceptionAsync<StubVerificationFailedException>(() =>
            client.VerifyStubCalledAsyncInternal(stubId, Between(2, 4), DateTime.UtcNow.AddSeconds(-1)));
        var result = ex.VerificationResultModel;

        // Assert
        Assert.IsFalse(result.Passed);
        Assert.AreEqual("Validation failed. Counted '5', but expected at most '4'.", result.Message);
    }

    private static RequestResultDto[] CreateResponse(string stubId, DateTime requestBeginTime, int numberOfRequests)
    {
        var result = new List<RequestResultDto>();
        for (var i = 0; i < numberOfRequests; i++)
        {
            result.Add(new RequestResultDto
            {
                ExecutingStubId = stubId,
                RequestBeginTime = requestBeginTime,
                RequestEndTime = requestBeginTime.AddSeconds(1),
                CorrelationId = Guid.NewGuid().ToString()
            });
        }

        return result.ToArray();
    }
}
