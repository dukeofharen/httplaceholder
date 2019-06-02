using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests
{
    [TestClass]
    public class HttPlaceholderClientModuleFacts
    {
        private readonly ServiceCollection _services = new ServiceCollection();

        [TestMethod]
        public void HttPlaceholderClientModule_AddHttPlaceholderClient_HappyFlow()
        {
            // Arrange
            _services.AddHttPlaceholderClient(settings =>
            {
                settings.BaseUrl = "http://localhost:5000";
                settings.Username = "user";
                settings.Password = "pass";
            });
            var provider = _services.BuildServiceProvider();

            // Act
            var client = provider.GetRequiredService<IHttPlaceholderClientFactory>();

            // Assert
            Assert.IsNotNull(client);
            Assert.IsNotNull(client.MetadataClient);
            Assert.IsNotNull(client.RequestClient);
            Assert.IsNotNull(client.StubClient);
            Assert.IsNotNull(client.TenantClient);
            Assert.IsNotNull(client.UserClient);
        }
    }
}
