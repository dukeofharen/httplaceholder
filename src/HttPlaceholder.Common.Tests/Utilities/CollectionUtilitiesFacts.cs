using System.Collections.Generic;
using HttPlaceholder.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Common.Tests.Utilities
{
    [TestClass]
    public class CollectionUtilitiesFacts
    {
        [TestMethod]
        public void AddOrReplaceCaseInsensitive_AddEntryWithSameCasing()
        {
            // Arrange
            var dictionary = new Dictionary<string, string> {{"Content-Type", "application/json"}};

            // Act
            dictionary.AddOrReplaceCaseInsensitive("Content-Type", "application/json");

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("application/json", dictionary["Content-Type"]);
        }

        [TestMethod]
        public void AddOrReplaceCaseInsensitive_AddEntryWithDifferentCasing()
        {
            // Arrange
            var dictionary = new Dictionary<string, string> {{"content-type", "application/json"}};

            // Act
            dictionary.AddOrReplaceCaseInsensitive("Content-Type", "application/json");

            // Assert
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("application/json", dictionary["Content-Type"]);
        }

        [TestMethod]
        public void AddOrReplaceCaseInsensitive_AddNewEntry()
        {
            // Arrange
            var dictionary = new Dictionary<string, string> {{"content-type", "application/json"}};

            // Act
            dictionary.AddOrReplaceCaseInsensitive("Accept", "text/xml");

            // Assert
            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual("application/json", dictionary["content-type"]);
            Assert.AreEqual("text/xml", dictionary["Accept"]);
        }
    }
}
