﻿using System.Net.Http;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

public abstract class BaseClientTest
{
    protected const string BaseUrl = "http://localhost:5000/";
    private readonly MockHttpMessageHandler _mockHttp = new();

    [TestCleanup]
    public void Cleanup()
    {
        _mockHttp.VerifyNoOutstandingExpectation();
        _mockHttp.VerifyNoOutstandingRequest();
    }

    protected HttpClient CreateHttpClient(Action<MockHttpMessageHandler> mockHttpAction = null)
    {
        mockHttpAction?.Invoke(_mockHttp);
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseUrl);
        return httpClient;
    }
}
