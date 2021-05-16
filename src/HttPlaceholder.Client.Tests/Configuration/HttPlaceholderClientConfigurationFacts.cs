using System;
using HttPlaceholder.Client.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests.Configuration
{
    [TestClass]
    public class HttPlaceholderClientConfigurationFacts
    {
        [TestMethod]
        public void RootUrl_NoValueSet_ShouldLeaveRootUrlEmpty()
        {
            // Arrange / Act
            var config = new HttPlaceholderClientConfiguration {RootUrl = string.Empty};

            // Assert
            Assert.IsNull(config.RootUrl);
        }

        [TestMethod]
        public void RootUrl_DoesntEndInSlash_ShouldAddSlash()
        {
            // Arrange / Act
            var config = new HttPlaceholderClientConfiguration {RootUrl = "http://localhost:5000"};

            // Assert
            Assert.AreEqual("http://localhost:5000/", config.RootUrl);
        }

        [TestMethod]
        public void RootUrl_EndsInSlash_ShouldNotAddExtraSlash()
        {
            // Arrange / Act
            var config = new HttPlaceholderClientConfiguration {RootUrl = "http://localhost:5000/"};

            // Assert
            Assert.AreEqual("http://localhost:5000/", config.RootUrl);
        }

        [TestMethod]
        public void Validate_RootUrlEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var config = new HttPlaceholderClientConfiguration {RootUrl = string.Empty};

            // Act
            var exception = Assert.ThrowsException<ArgumentException>(() => config.Validate());

            // Assert
            Assert.AreEqual("No value set for RootUrl in HttPlaceholder configuration.", exception.Message);
        }
    }
}
