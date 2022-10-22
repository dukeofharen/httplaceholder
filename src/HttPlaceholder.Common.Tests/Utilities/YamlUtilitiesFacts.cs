using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class YamlUtilitiesFacts
{
    [TestMethod]
    public void Parse_HappyFlow()
    {
        // Arrange
        const string yaml = "value: val1";

        // Act
        var result = YamlUtilities.Parse<TestModel>(yaml);

        // Assert
        Assert.AreEqual("val1", result.Value);
    }

    [TestMethod]
    public void Parse_HappyFlow_ShouldHandleUnmappedFields()
    {
        // Arrange
        const string yaml = "value: val1\nunmappedField: 123";

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
        Assert.AreEqual("value: val1\n", result);
    }

    private class TestModel
    {
        public string Value { get; set; }
    }
}
