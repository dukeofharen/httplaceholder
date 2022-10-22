using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Tests.HostedServices;

[TestClass]
public class HostedServiceModuleFacts
{
    [TestMethod]
    public void AddHostedServices_CleanOldRequestsInBackgroundJobIsFalse_ShouldNotRegisterCleanOldRequestsJob()
    {
        // Arrange
        var args = new Dictionary<string, string> {{"Storage:CleanOldRequestsInBackgroundJob", "false"}};
        var services = new ServiceCollection();

        // Act
        services.AddHostedServices(BuildConfiguration(args));

        // Assert
        Assert.AreEqual(0, services.Count);
    }

    [TestMethod]
    public void AddHostedServices_CleanOldRequestsInBackgroundJobIsTrue_ShouldRegisterCleanOldRequestsJob()
    {
        // Arrange
        var args = new Dictionary<string, string> {{"Storage:CleanOldRequestsInBackgroundJob", "true"}};
        var services = new ServiceCollection();

        // Act
        services.AddHostedServices(BuildConfiguration(args));

        // Assert
        Assert.AreEqual(2, services.Count);
        Assert.IsTrue(services.All(s => s.ImplementationType == typeof(CleanOldRequestsJob)));
    }

    private static IConfiguration BuildConfiguration(IDictionary<string, string> dict) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
}
