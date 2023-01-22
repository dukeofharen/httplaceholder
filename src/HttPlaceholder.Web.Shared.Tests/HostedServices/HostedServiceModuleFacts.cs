using HttPlaceholder.Web.Shared.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Web.Shared.Tests.HostedServices;

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
        Assert.AreEqual(1, services.Count);
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
        Assert.AreEqual(3, services.Count);
        Assert.IsTrue(services.Any(s => s.ImplementationType == typeof(CleanOldRequestsJob)));
    }

    private static IConfiguration BuildConfiguration(IDictionary<string, string> dict) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
}
