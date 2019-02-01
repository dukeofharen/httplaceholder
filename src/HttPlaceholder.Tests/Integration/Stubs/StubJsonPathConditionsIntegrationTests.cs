using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    [TestClass]
    public class StubJsonPathConditionsIntegrationTests : StubIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            InitializeStubIntegrationTest("integration.yml");
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanupIntegrationTest();
        }

        [TestMethod]
        public async Task StubIntegration_RegularPut_Json_ValidateJsonPath_HappyFlow()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}users";
            string body = @"{
  ""firstName"": ""John"",
  ""lastName"" : ""doe"",
  ""age""      : 26,
  ""address""  : {
    ""streetAddress"": ""naist street"",
    ""city""         : ""Nara"",
    ""postalCode""   : ""630-0192""
  },
  ""phoneNumbers"": [
    {
      ""type""  : ""iPhone"",
      ""number"": ""0123-4567-8888""
    },
    {
      ""type""  : ""home"",
      ""number"": ""0123-4567-8910""
    }
  ]
}";
            var request = new HttpRequestMessage
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Put,
                RequestUri = new Uri(url)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
                Assert.IsTrue(string.IsNullOrEmpty(content));
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularPut_Json_ValidateJsonPath_StubNotFound()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}users";
            string body = @"{
  ""firstName"": ""John"",
  ""lastName"" : ""doe"",
  ""age""      : 26,
  ""address""  : {
    ""streetAddress"": ""naist street"",
    ""city""         : ""Nara"",
    ""postalCode""   : ""630-0192""
  },
  ""phoneNumbers"": [
    {
      ""type""  : ""Android"",
      ""number"": ""0123-4567-8888""
    },
    {
      ""type""  : ""home"",
      ""number"": ""0123-4567-8910""
    }
  ]
}";
            var request = new HttpRequestMessage
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Put,
                RequestUri = new Uri(url)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
                Assert.IsTrue(string.IsNullOrEmpty(content));
            }
        }
    }
}
