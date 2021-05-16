using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests
{
    [TestClass]
    public class HttPlaceholderClientFactoryFacts
    {
        [TestMethod]
        public void CreateHttPlaceholderClient_ShouldReuseHttpClient()
        {
            // Arrange
            var config = new HttPlaceholderClientConfiguration {RootUrl = "http://localhost:5000"};

            // Act
            var client1 = (HttPlaceholderClient)HttPlaceholderClientFactory.CreateHttPlaceholderClient(config);
            var client2 = (HttPlaceholderClient)HttPlaceholderClientFactory.CreateHttPlaceholderClient(config);

            // Assert
            Assert.AreEqual(client1.HttpClient, client2.HttpClient);
        }

        [TestMethod]
        public void CreateHttPlaceholderClient_ShouldSetBaseUrl()
        {
            // Arrange
            var config = new HttPlaceholderClientConfiguration {RootUrl = "http://localhost:5000"};

            // Act
            var client = (HttPlaceholderClient)HttPlaceholderClientFactory.CreateHttPlaceholderClient(config);

            // Assert
            Assert.AreEqual("http://localhost:5000/", client.HttpClient.BaseAddress.OriginalString);
        }
    }
}
