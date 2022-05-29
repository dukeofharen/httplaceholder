using System;
using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.Tests.StubExecution.Utilities;

[TestClass]
public class StringConditionUtilitiesFacts
{
    [TestMethod]
    public void ConvertCondition_ConditionIsDictionary_ShouldConvert()
    {
        // Arrange
        var dict = new Dictionary<object, object> {{"equals", "/path"}, {"notcontainsci", "somestring"}};

        // Act
        var result = StringConditionUtilities.ConvertCondition(dict);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("/path", result.StringEquals);
        Assert.AreEqual("somestring", result.NotContainsCi);
    }

    [TestMethod]
    public void ConvertCondition_ConditionIsJObject_ShouldConvert()
    {
        // Arrange
        var jsonString = @"{""equals"": ""/path"", ""notcontainsci"": ""somestring""}";

        // Act
        var result = StringConditionUtilities.ConvertCondition(JObject.Parse(jsonString));

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("/path", result.StringEquals);
        Assert.AreEqual("somestring", result.NotContainsCi);
    }

    [TestMethod]
    public void ConvertCondition_ConditionIsUnrecognized_ShouldThrowInvalidOperationException()
    {
        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() => StringConditionUtilities.ConvertCondition(1234));
    }
}
