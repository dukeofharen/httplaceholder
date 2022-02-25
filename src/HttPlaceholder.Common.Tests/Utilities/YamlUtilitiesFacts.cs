using HttPlaceholder.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class YamlUtilitiesFacts
{
    [TestMethod]
    public void Parse_HappyFlow()
    {
        // Arrange
        const string yaml = "Value: val1";

        // Act
        var result = YamlUtilities.Parse<TestModel>(yaml);

        // Assert
        Assert.AreEqual("val1", result.Value);
    }

    [TestMethod]
    public void Serialize_HappyFlow()
    {
        // Arrange
        var input = new TestModel {Value = "val1"};

        // Act
        var result = YamlUtilities.Serialize(input);

        // Assert
        Assert.AreEqual("Value: val1\n", result);
    }

    private class TestModel
    {
        public string Value { get; set; }
    }
}
