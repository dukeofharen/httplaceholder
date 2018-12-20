using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using HttPlaceholder.Client.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests
{
    [TestClass]
    public class HttPlaceholderClientFacts
    {
        private const string RootUrl = "http://localhost:5000";
        private const string Username = "user";
        private const string Password = "pass";
        private HttPlaceholderClient _client;

        [TestInitialize]
        public void Initialize()
        {
            _client = new HttPlaceholderClient(new HttPlaceholderSettingsModel
            {
                RootUrl = RootUrl,
                Username = Username,
                Password = Password
            });
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetUserAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                httpTest.RespondWithJson(new
                {
                    username = Username
                });

                // act
                await _client.GetUserAsync(Username, Password);

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/users/{Username}")
                    .WithVerb(HttpMethod.Get)
                    .WithBasicAuth(Username, Password);
            }
        }
    }
}
