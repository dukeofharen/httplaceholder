using System;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class StringCheckerFacts
{
    private readonly StringChecker _checker = new();

    [TestMethod]
    public void CheckString_InputIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentNullException>(() => _checker.CheckString(null, null, out _));

        // Assert
        Assert.IsTrue(exception.Message.Contains("input"));
    }

    [TestMethod]
    public void CheckString_ConditionIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentNullException>(() => _checker.CheckString("text", null, out _));

        // Assert
        Assert.IsTrue(exception.Message.Contains("condition"));
    }

    [TestMethod]
    public void CheckString_ConditionIsString_RegexSucceeds_ShouldReturnTrue()
    {
        // Act
        var result = _checker.CheckString("text", "text", out var outputForLogging);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
    }

    [TestMethod]
    public void CheckString_ConditionIsString_RegexFails_ShouldReturnFalse()
    {
        // Act
        var result = _checker.CheckString("text", "incorrectregex", out var outputForLogging);

        // Assert
        Assert.IsFalse(result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
    }

    [DataTestMethod]
    [DataRow("teststring", "teststring", true)]
    [DataRow("teststring1", "teststring", false)]
    [DataRow("teststrin", "teststring", false)]
    [DataRow("teststring", "teststring1", false)]
    [DataRow("Teststring", "teststring", false)]
    public void CheckString_StringEquals(string input, string stringEquals, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {StringEquals = stringEquals});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "teststring", true)]
    [DataRow("Teststring", "teststring", true)]
    [DataRow("Teststring", "Teststring", true)]
    [DataRow("Teststring", "Teststring1", false)]
    [DataRow("Teststring1", "Teststring", false)]
    public void CheckString_StringEqualsCi(string input, string stringEqualsCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {StringEqualsCi = stringEqualsCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "teststring", false)]
    [DataRow("teststring1", "teststring", true)]
    [DataRow("teststrin", "teststring", true)]
    [DataRow("teststring", "teststring1", true)]
    [DataRow("Teststring", "teststring", true)]
    public void CheckString_StringNotEquals(string input, string stringNotEquals, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {StringNotEquals = stringNotEquals});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "teststring", false)]
    [DataRow("Teststring", "teststring", false)]
    [DataRow("Teststring", "Teststring", false)]
    [DataRow("Teststring", "Teststring1", true)]
    [DataRow("Teststring1", "Teststring", true)]
    public void CheckString_StringNotEqualsCi(string input, string stringNotEqualsCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {StringNotEqualsCi = stringNotEqualsCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", true)]
    [DataRow("teststring", "string", true)]
    [DataRow("Teststring", "string", true)]
    [DataRow("Teststring", "test", false)]
    [DataRow("Teststring", "bla", false)]
    public void CheckString_Contains(string input, string contains, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {Contains = contains});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", true)]
    [DataRow("teststring", "string", true)]
    [DataRow("Teststring", "string", true)]
    [DataRow("Teststring", "test", true)]
    [DataRow("Teststring", "bla", false)]
    public void CheckString_ContainsCi(string input, string containsCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {ContainsCi = containsCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", false)]
    [DataRow("teststring", "string", false)]
    [DataRow("Teststring", "string", false)]
    [DataRow("Teststring", "test", true)]
    [DataRow("Teststring", "bla", true)]
    public void CheckString_NotContains(string input, string notContains, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {NotContains = notContains});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", false)]
    [DataRow("teststring", "string", false)]
    [DataRow("Teststring", "string", false)]
    [DataRow("Teststring", "test", false)]
    [DataRow("Teststring", "bla", true)]
    public void CheckString_NotContainsCi(string input, string notContainsCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {NotContainsCi = notContainsCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", true)]
    [DataRow("teststring", "tes", true)]
    [DataRow("teststring", "Test", false)]
    [DataRow("teststring", "string", false)]
    public void CheckString_StartsWith(string input, string startsWith, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {StartsWith = startsWith});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", true)]
    [DataRow("teststring", "tes", true)]
    [DataRow("teststring", "Test", true)]
    [DataRow("teststring", "string", false)]
    public void CheckString_StartsWithCi(string input, string startsWithCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {StartsWithCi = startsWithCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", false)]
    [DataRow("teststring", "tes", false)]
    [DataRow("teststring", "Test", true)]
    [DataRow("teststring", "string", true)]
    public void CheckString_DoesNotStartWith(string input, string doesNotStartWith, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {DoesNotStartWith = doesNotStartWith});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", false)]
    [DataRow("teststring", "tes", false)]
    [DataRow("teststring", "Test", false)]
    [DataRow("teststring", "string", true)]
    public void CheckString_DoesNotStartWithCi(string input, string doesNotStartWithCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {DoesNotStartWithCi = doesNotStartWithCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "string", true)]
    [DataRow("teststring", "test", false)]
    [DataRow("teststring", "strinG", false)]
    [DataRow("teststring", "String", false)]
    [DataRow("teststring", "strin", false)]
    [DataRow("teststring", "tring", true)]
    public void CheckString_EndsWith(string input, string endsWith, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {EndsWith = endsWith});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "string", true)]
    [DataRow("teststring", "test", false)]
    [DataRow("teststring", "strinG", true)]
    [DataRow("teststring", "String", true)]
    [DataRow("teststring", "strin", false)]
    [DataRow("teststring", "tring", true)]
    public void CheckString_EndsWithCi(string input, string endsWithCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {EndsWithCi = endsWithCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "string", false)]
    [DataRow("teststring", "test", true)]
    [DataRow("teststring", "strinG", true)]
    [DataRow("teststring", "String", true)]
    [DataRow("teststring", "strin", true)]
    [DataRow("teststring", "tring", false)]
    public void CheckString_DoesNotEndWith(string input, string doesNotEndWith, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {DoesNotEndWith = doesNotEndWith});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "string", false)]
    [DataRow("teststring", "test", true)]
    [DataRow("teststring", "strinG", false)]
    [DataRow("teststring", "String", false)]
    [DataRow("teststring", "strin", true)]
    [DataRow("teststring", "tring", false)]
    public void CheckString_DoesNotEndWithCi(string input, string doesNotEndWithCi, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {DoesNotEndWithCi = doesNotEndWithCi});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "teststring", true)]
    [DataRow("teststring", "^([a-z]*)$", true)]
    [DataRow("teststring", "^([0-9]*)$", false)]
    public void CheckString_Regex(string input, string regex, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {Regex = regex});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "teststring", false)]
    [DataRow("teststring", "^([a-z]*)$", false)]
    [DataRow("teststring", "^([0-9]*)$", true)]
    public void CheckString_RegexNoMatches(string input, string regexNoMatches, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {RegexNoMatches = regexNoMatches});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", -10, false)]
    [DataRow("teststring", 0, true)]
    [DataRow("teststring", 1, true)]
    [DataRow("teststring", 9, true)]
    [DataRow("teststring", 10, true)]
    [DataRow("teststring", 11, false)]
    [DataRow("teststring", 20, false)]
    public void CheckString_MinLength(string input, int minLength, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {MinLength = minLength});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", -10, false)]
    [DataRow("teststring", 0, false)]
    [DataRow("teststring", 1, false)]
    [DataRow("teststring", 9, false)]
    [DataRow("teststring", 10, true)]
    [DataRow("teststring", 11, true)]
    [DataRow("teststring", 20, true)]
    public void CheckString_MaxLength(string input, int maxLength, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {MaxLength = maxLength});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", -10, 10, false)]
    [DataRow("teststring", 0, 10, true)]
    [DataRow("teststring", 1, 10, true)]
    [DataRow("teststring", 1, 11, true)]
    [DataRow("teststring", 1, 9, false)]
    [DataRow("teststring", 5, 5, false)]
    [DataRow("teststring", 9, 20, true)]
    [DataRow("teststring", 10, 20, true)]
    [DataRow("teststring", 11, 20, false)]
    public void CheckString_MinAndMaxLength(string input, int minLength, int maxLength, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {MinLength = minLength, MaxLength = maxLength});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", -10, false)]
    [DataRow("teststring", 0, false)]
    [DataRow("teststring", 1, false)]
    [DataRow("teststring", 9, false)]
    [DataRow("teststring", 10, true)]
    [DataRow("teststring", 11, false)]
    [DataRow("teststring", 20, false)]
    public void ExactLength(string input, int exactLength, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {ExactLength = exactLength});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    [DataTestMethod]
    [DataRow("teststring", "test", "string", true)]
    [DataRow("teststring", "test", "tring", true)]
    [DataRow("teststring", "tri", "tring", true)]
    [DataRow("teststring", "tri", "String", false)]
    [DataRow("teststring", "testt", "tring", false)]
    public void CheckString_MultipleChecks(string input, string contains, string endsWith, bool shouldSucceed)
    {
        // Arrange
        var condition = Convert(new StubConditionStringCheckingModel {Contains = contains, EndsWith = endsWith});

        // Act
        var result = _checker.CheckString(input, condition, out var outputForLogging);

        // Assert
        Assert.AreEqual(shouldSucceed, result);
        if (shouldSucceed)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(outputForLogging));
        }
        else
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputForLogging));
        }
    }

    private static JObject Convert(StubConditionStringCheckingModel input)
    {
        var json = JsonConvert.SerializeObject(input);
        return JObject.Parse(json);
    }
}
