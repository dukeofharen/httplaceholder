using System.Collections.Generic;
using HttPlaceholder.Client.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Client.Tests;

[TestClass]
public class HttPlaceholderClientModuleFacts
{
    [TestMethod]
    public void AddHttPlaceholderClient_DotNetConfig_ShouldSuccessfullyRegisterClient()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            { "RootUrl", "http://localhost:5000" }, { "Username", "username" }, { "Password", "password" }
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        var services = new ServiceCollection();
        services.AddHttPlaceholderClient(config);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var client = (HttPlaceholderClient)serviceProvider.GetRequiredService<IHttPlaceholderClient>();

        // Assert
        Assert.AreEqual("http://localhost:5000/", client.HttpClient.BaseAddress.OriginalString);
    }

    [TestMethod]
    public void AddHttPlaceholderClient_ConfigAsAction_ShouldSuccessfullyRegisterClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHttPlaceholderClient(config =>
        {
            config.RootUrl = "http://localhost:5000";
            config.Username = "username";
            config.Password = "password";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var client = (HttPlaceholderClient)serviceProvider.GetRequiredService<IHttPlaceholderClient>();

        // Assert
        Assert.AreEqual("http://localhost:5000/", client.HttpClient.BaseAddress.OriginalString);
    }
}
