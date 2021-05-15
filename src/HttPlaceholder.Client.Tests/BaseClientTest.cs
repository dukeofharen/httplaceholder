using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests
{
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

        public HttpClient CreateHttpClient(Action<MockHttpMessageHandler> mockHttpAction = null)
        {
            mockHttpAction?.Invoke(_mockHttp);
            var httpClient = _mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);
            return httpClient;
        }
    }
}
