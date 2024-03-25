using System.Collections.Generic;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class CollectionUtilitiesFacts
{
    [TestMethod]
    public void AddOrReplaceCaseInsensitive_AddEntryWithSameCasing()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { HeaderKeys.ContentType, MimeTypes.JsonMime } };

        // Act
        dictionary.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, MimeTypes.TextMime);

        // Assert
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual(MimeTypes.TextMime, dictionary[HeaderKeys.ContentType]);
    }

    [TestMethod]
    public void AddOrReplaceCaseInsensitive_AddEntryWithDifferentCasing()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { HeaderKeys.ContentType, MimeTypes.JsonMime } };

        // Act
        dictionary.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, MimeTypes.TextMime);

        // Assert
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual(MimeTypes.TextMime, dictionary[HeaderKeys.ContentType]);
    }

    [TestMethod]
    public void AddOrReplaceCaseInsensitive_AddEntryWithDifferentCasing_DoNotReplaceExistingValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { HeaderKeys.ContentType, MimeTypes.JsonMime } };

        // Act
        dictionary.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, MimeTypes.TextMime, false);

        // Assert
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual(MimeTypes.JsonMime, dictionary[HeaderKeys.ContentType]);
    }

    [TestMethod]
    public void AddOrReplaceCaseInsensitive_AddNewEntry()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { HeaderKeys.ContentType, MimeTypes.JsonMime } };

        // Act
        dictionary.AddOrReplaceCaseInsensitive("Accept", MimeTypes.XmlTextMime);

        // Assert
        Assert.AreEqual(2, dictionary.Count);
        Assert.AreEqual(MimeTypes.JsonMime, dictionary[HeaderKeys.ContentType]);
        Assert.AreEqual(MimeTypes.XmlTextMime, dictionary["Accept"]);
    }

    [DataTestMethod]
    [DataRow("var1", "value1")]
    [DataRow("VAr1", "value1")]
    [DataRow("var2", "value2")]
    [DataRow("var3", null)]
    public void CaseInsensitiveSearch_ShouldReturnCorrectValue(string key, string expectedValue)
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "var1", "value1" }, { "var2", "value2" } };

        // Act
        var result = dictionary.CaseInsensitiveSearch(key);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    [DataTestMethod]
    [DataRow("var1", "value1", true)]
    [DataRow("VAr1", "value1", true)]
    [DataRow("var2", "value2", true)]
    [DataRow("var3", null, false)]
    public void TryGetCaseInsensitive_ShouldReturnCorrectValue(string key, string expectedValue, bool expectedResult)
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "var1", "value1" }, { "var2", "value2" } };

        // Act
        var result = dictionary.TryGetCaseInsensitive(key, out var foundValue);

        // Assert
        Assert.AreEqual(expectedValue, foundValue);
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("key1", true)]
    [DataRow("Key1", true)]
    [DataRow("KEY1", true)]
    [DataRow("key11", false)]
    [DataRow("key3", false)]
    public void ContainsKeyCaseInsensitive_HappyFlow(string input, bool expectedResult)
    {
        // Arrange
        var dict = new Dictionary<string, string> { { "key1", "val1" }, { "key2", "val2" } };

        // Act
        var result = dict.ContainsKeyCaseInsensitive(input);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
