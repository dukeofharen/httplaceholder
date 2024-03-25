using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Utilities;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.Tests.StubExecution.Utilities;

[TestClass]
public class ConversionUtilitiesFacts
{
    [TestMethod]
    public void Convert_ConditionIsDictionary_ShouldConvert()
    {
        // Arrange
        var dict = new Dictionary<object, object> { { "equals", "/path" }, { "notcontainsci", "somestring" } };

        // Act
        var result = ConversionUtilities.Convert<StubConditionStringCheckingModel>(dict);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("/path", result.StringEquals);
        Assert.AreEqual("somestring", result.NotContainsCi);
    }

    [TestMethod]
    public void Convert_ConditionIsJObject_ShouldConvert()
    {
        // Arrange
        const string jsonString = """{"equals": "/path", "notcontainsci": "somestring"}""";

        // Act
        var result = ConversionUtilities.Convert<StubConditionStringCheckingModel>(JObject.Parse(jsonString));

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("/path", result.StringEquals);
        Assert.AreEqual("somestring", result.NotContainsCi);
    }

    [TestMethod]
    public void Convert_ConditionIsModel_ShouldReturnAsIs()
    {
        // Arrange
        var model = new StubConditionStringCheckingModel { StringEquals = "123" };

        // Act
        var result = ConversionUtilities.Convert<StubConditionStringCheckingModel>(model);

        // Assert
        Assert.AreEqual(model, result);
    }

    [TestMethod]
    public void Convert_ConditionIsUnrecognized_ShouldThrowInvalidOperationException() =>
        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            ConversionUtilities.Convert<StubConditionStringCheckingModel>(1234));

    [TestMethod]
    public void ConvertEnumerable_InputIsJArray_ShouldConvert()
    {
        // Arrange
        var input = JArray.Parse("""["string1", "string2"]""");

        // Act
        var result = ConversionUtilities.ConvertEnumerable<string>(input).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("string1", result[0]);
        Assert.AreEqual("string2", result[1]);
    }

    [TestMethod]
    public void ConvertEnumerable_InputIsObjectList_ShouldConvert()
    {
        // Arrange
        var input = new List<object> { "string1", "string2" };

        // Act
        var result = ConversionUtilities.ConvertEnumerable<string>(input).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("string1", result[0]);
        Assert.AreEqual("string2", result[1]);
    }

    [TestMethod]
    public void ConvertEnumerable_InputIsUnrecognized_ShouldThrowInvalidOperationException() =>
        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() => ConversionUtilities.Convert<string>(123));

    [DataTestMethod]
    [DataRow(3, 3)]
    [DataRow((long)3, 3)]
    [DataRow("3", 3)]
    [DataRow(false, null)]
    public void ParseInteger_HappyFlow(object input, object expectedResult)
    {
        // Act
        var result = ConversionUtilities.ParseInteger(input);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
