using HttPlaceholder.Infrastructure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Infrastructure.Tests.Configuration;

[TestClass]
public class ConfigurationHelperFacts
{
    [TestMethod]
    public void GetConfigKeyMetadata_HappyFlow()
    {
        // Act
        var result = ConfigurationHelper.GetConfigKeyMetadata();

        // Assert
        Assert.AreEqual(23, result.Count);
    }
}
