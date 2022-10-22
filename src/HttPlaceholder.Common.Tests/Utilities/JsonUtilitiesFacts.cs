using HttPlaceholder.Common.Utilities;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class JsonUtilitiesFacts
{
    [TestMethod]
    public void ConvertFoundValue_TokenIsJValue_ShouldReturnValueCorrectly()
    {
        // Arrange
        var jValue = JValue.CreateString("value");

        // Act
        var result = JsonUtilities.ConvertFoundValue(jValue);

        // Assert
        Assert.AreEqual("value", result);
    }

    [TestMethod]
    public void ConvertFoundValue_TokenIsJArray_ArrayHasValues_ShouldReturnValueCorrectly()
    {
        // Arrange
        var jArray = JArray.FromObject(new[] {"val1", "val2"});

        // Act
        var result = JsonUtilities.ConvertFoundValue(jArray);

        // Assert
        Assert.AreEqual("val1", result);
    }

    [TestMethod]
    public void ConvertFoundValue_TokenIsJArray_ArrayHasNoValues_ShouldReturnEmptyString()
    {
        // Arrange
        var jArray = JArray.FromObject(Array.Empty<string>());

        // Act
        var result = JsonUtilities.ConvertFoundValue(jArray);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ConvertFoundValue_TokenIsJObject_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var jObject = JObject.FromObject(new { });

        // Act
        var exception =
            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonUtilities.ConvertFoundValue(jObject));

        // Assert
        Assert.AreEqual("JSON type 'Newtonsoft.Json.Linq.JObject' not supported.", exception.Message);
    }
}
