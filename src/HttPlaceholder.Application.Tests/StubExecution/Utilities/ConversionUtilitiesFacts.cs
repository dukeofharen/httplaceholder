using System.Collections.Generic;
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
        var dict = new Dictionary<object, object> {{"equals", "/path"}, {"notcontainsci", "somestring"}};

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
        const string jsonString = @"{""equals"": ""/path"", ""notcontainsci"": ""somestring""}";

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
        var model = new StubConditionStringCheckingModel {StringEquals = "123"};

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
