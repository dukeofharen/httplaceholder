using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiStubGenerationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_StubGeneration_HappyFlow()
        {
            // Arrange
            ClientDataResolverMock
                .Setup(m => m.GetClientIp())
                .Returns("127.0.0.1");

            ClientDataResolverMock
                .Setup(m => m.GetHost())
                .Returns("localhost");

            // Do a call to a non-existant stub
            var response = await Client.SendAsync(CreateTestStubRequest());
            var correlationId = response.Headers.Single(h => h.Key == "X-HttPlaceholder-Correlation").Value.Single();

            // Register a new stub for the failed request
            var url = $"{BaseAddress}ph-api/requests/{correlationId}/stubs";
            var apiRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };
            response = await Client.SendAsync(apiRequest);
            response.EnsureSuccessStatusCode();

            // Do the original stub request again; it should succeed now
            response = await Client.SendAsync(CreateTestStubRequest());
            response.EnsureSuccessStatusCode();

            // Check the actual added stub
            var addedStub = StubSource._stubModels.Single();
            Assert.AreEqual("/test123", addedStub.Conditions.Url.Path);

            Assert.AreEqual("val1", addedStub.Conditions.Url.Query["query1"]);
            Assert.AreEqual("val2", addedStub.Conditions.Url.Query["query2"]);

            Assert.AreEqual("POST", addedStub.Conditions.Method);

            var formDict = addedStub.Conditions.Form.ToDictionary(f => f.Key, f => f.Value);
            Assert.AreEqual("val1", formDict["form1"]);
            Assert.AreEqual("val2", formDict["form2"]);

            Assert.AreEqual("abc123", addedStub.Conditions.Headers["X-Api-Key"]);

            Assert.AreEqual("127.0.0.1", addedStub.Conditions.ClientIp);

            Assert.IsFalse(addedStub.Conditions.Url.IsHttps.HasValue &&addedStub.Conditions.Url.IsHttps.Value);

            Assert.AreEqual("localhost", addedStub.Conditions.Host);

            Assert.AreEqual("duco", addedStub.Conditions.BasicAuthentication.Username);
            Assert.AreEqual("pass", addedStub.Conditions.BasicAuthentication.Password);
        }

        private HttpRequestMessage CreateTestStubRequest()
        {
            var clientRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseAddress}test123?query1=val1&query2=val2"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"form1", "val1"}, {"form2", "val2"}
                })
            };
            clientRequest.Headers.Add("X-Api-Key", "abc123");

            var auth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("duco:pass"));
            clientRequest.Headers.Add("Authorization", auth);
            return clientRequest;
        }
    }
}
