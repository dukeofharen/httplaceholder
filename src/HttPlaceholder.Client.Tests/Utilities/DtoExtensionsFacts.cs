using static HttPlaceholder.Client.Utilities.DtoExtensions;

namespace HttPlaceholder.Client.Tests.Utilities;

[TestClass]
public class DtoExtensionsFacts
{
    [TestMethod]
    public void StringEquals_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = StringEquals(input);

        // Assert
        Assert.AreEqual(input, result.StringEquals);
    }

    [TestMethod]
    public void StringEqualsCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = StringEqualsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.StringEqualsCi);
    }

    [TestMethod]
    public void StringNotEquals_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = StringNotEquals(input);

        // Assert
        Assert.AreEqual(input, result.StringNotEquals);
    }

    [TestMethod]
    public void StringNotEqualsCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = StringNotEqualsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.StringNotEqualsCi);
    }

    [TestMethod]
    public void Contains_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = Contains(input);

        // Assert
        Assert.AreEqual(input, result.Contains);
    }

    [TestMethod]
    public void ContainsCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = ContainsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.ContainsCi);
    }

    [TestMethod]
    public void NotContains_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = NotContains(input);

        // Assert
        Assert.AreEqual(input, result.NotContains);
    }

    [TestMethod]
    public void NotContainsCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = NotContainsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.NotContainsCi);
    }

    [TestMethod]
    public void StartsWith_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = StartsWith(input);

        // Assert
        Assert.AreEqual(input, result.StartsWith);
    }

    [TestMethod]
    public void StartsWithCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = StartsWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.StartsWithCi);
    }

    [TestMethod]
    public void DoesNotStartWith_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = DoesNotStartWith(input);

        // Assert
        Assert.AreEqual(input, result.DoesNotStartWith);
    }

    [TestMethod]
    public void DoesNotStartWithCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = DoesNotStartWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.DoesNotStartWithCi);
    }

    [TestMethod]
    public void EndsWith_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = EndsWith(input);

        // Assert
        Assert.AreEqual(input, result.EndsWith);
    }

    [TestMethod]
    public void EndsWithCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = EndsWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.EndsWithCi);
    }

    [TestMethod]
    public void DoesNotEndWith_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = DoesNotEndWith(input);

        // Assert
        Assert.AreEqual(input, result.DoesNotEndWith);
    }

    [TestMethod]
    public void DoesNotEndWithCaseInsensitive_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = DoesNotEndWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, result.DoesNotEndWithCi);
    }

    [TestMethod]
    public void Regex_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = Regex(input);

        // Assert
        Assert.AreEqual(input, result.Regex);
    }

    [TestMethod]
    public void RegexNoMatches_HappyFlow()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = RegexNoMatches(input);

        // Assert
        Assert.AreEqual(input, result.RegexNoMatches);
    }

    [TestMethod]
    public void MinLength_HappyFlow()
    {
        // Arrange
        const int input = 2;

        // Act
        var result = MinLength(input);

        // Assert
        Assert.AreEqual(input, result.MinLength);
    }

    [TestMethod]
    public void MaxLength_HappyFlow()
    {
        // Arrange
        const int input = 2;

        // Act
        var result = MaxLength(input);

        // Assert
        Assert.AreEqual(input, result.MaxLength);
    }

    [TestMethod]
    public void ExactLength_HappyFlow()
    {
        // Arrange
        const int input = 2;

        // Act
        var result = ExactLength(input);

        // Assert
        Assert.AreEqual(input, result.ExactLength);
    }
}
