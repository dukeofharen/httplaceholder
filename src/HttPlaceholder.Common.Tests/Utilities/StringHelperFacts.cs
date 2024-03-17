using System.Collections.Generic;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class StringHelperFacts
{
    public static IEnumerable<object> GetFirstNonWhitespaceString_TestData =>
        new[]
        {
            new object[] { new[] { "1", "2", "3" }, "1" }, new object[] { new[] { "", "2", "3" }, "2" },
            new object[] { new[] { "", null, "3" }, "3" }, new object[] { new[] { "", null, null }, null }
        };

    [TestMethod]
    public void IsRegexMatchOrSubstring_IsNotAMatch_ShouldReturnFalse()
    {
        // Arrange
        const string input = "input";
        const string regex = "input1";

        // Act
        var result = StringHelper.IsRegexMatchOrSubstring(input, regex);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsRegexMatchOrSubstring_IsAMatch_ShouldReturnTrue()
    {
        // Arrange
        const string input = "input";
        const string regex = "input";

        // Act
        var result = StringHelper.IsRegexMatchOrSubstring(input, regex);

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("input-", "-", "input-")]
    [DataRow("input", "-", "input-")]
    public void EnsureEndsWith_HappyFlow(string input, string append, string expectedResult)
    {
        // Act
        var result = input.EnsureEndsWith(append);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("input-", '-', "input")]
    [DataRow("input", '-', "input")]
    [DataRow("--input-", '-', "--input")]
    [DataRow("--input----", '-', "--input")]
    public void EnsureDoesntEndWith_HappyFlow(string input, char character, string expectedResult)
    {
        // Act
        var result = input.EnsureDoesntEndWith(character);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("-input", "-", "-input")]
    [DataRow("input", "-", "-input")]
    public void EnsureStartsWith_HappyFlow(string input, string append, string expectedResult)
    {
        // Act
        var result = input.EnsureStartsWith(append);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void CountNumberOfNonWhitespaceStrings_NoInput_ShouldReturn0()
    {
        // Act
        var result = StringHelper.CountNumberOfNonWhitespaceStrings();

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void CountNumberOfNonWhitespaceStrings_Input_ShouldReturnNumberOfNonWhitespaceOrNull()
    {
        // Act
        var result = StringHelper.CountNumberOfNonWhitespaceStrings("", null, "input1", null, "input2");

        // Assert
        Assert.AreEqual(2, result);
    }

    [DataTestMethod]
    [DataRow("split\nthis\ntext")]
    [DataRow("split\r\nthis\r\ntext")]
    [DataRow("split\rthis\rtext")]
    [DataRow("split\nthis\r\ntext")]
    public void SplitNewlines_HappyFlow(string input)
    {
        // Act
        var result = input.SplitNewlines();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual("split", result[0]);
        Assert.AreEqual("this", result[1]);
        Assert.AreEqual("text", result[2]);
    }

    [TestMethod]
    [DynamicData(nameof(GetFirstNonWhitespaceString_TestData))]
    public void GetFirstNonWhitespaceString_HappyFlow(string[] input, string expectedOutput)
    {
        // Act
        var result = StringHelper.GetFirstNonWhitespaceString(input);

        // Assert
        Assert.AreEqual(expectedOutput, result);
    }
}
