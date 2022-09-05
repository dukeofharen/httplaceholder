using HttPlaceholder.Infrastructure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Infrastructure.Tests.Configuration;

[TestClass]
public class ConfigurationHelperFacts
{
    private readonly ConfigurationHelper _configurationHelper = new();

    [TestMethod]
    public void GetConfigKeyMetadata_HappyFlow()
    {
        // Act
        var result = _configurationHelper.GetConfigKeyMetadata();

        // Assert
        Assert.AreEqual(23, result.Count);
    }
}
