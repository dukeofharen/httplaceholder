using HttPlaceholder.Client.StubBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests.StubBuilders;

[TestClass]
public class StringCheckingDtoBuilderFacts
{
    private readonly StringCheckingDtoBuilder _builder = StringCheckingDtoBuilder.Begin();

    [TestMethod]
    public void StringEquals()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.StringEquals(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().StringEquals);
    }

    [TestMethod]
    public void StringEqualsCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.StringEqualsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().StringEqualsCi);
    }

    [TestMethod]
    public void StringNotEquals()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.StringNotEquals(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().StringNotEquals);
    }

    [TestMethod]
    public void StringNotEqualsCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.StringNotEqualsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().StringNotEqualsCi);
    }

    [TestMethod]
    public void Contains()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.Contains(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().Contains);
    }

    [TestMethod]
    public void ContainsCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.ContainsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().ContainsCi);
    }

    [TestMethod]
    public void NotContains()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.NotContains(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().NotContains);
    }

    [TestMethod]
    public void NotContainsCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.NotContainsCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().NotContainsCi);
    }

    [TestMethod]
    public void StartsWith()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.StartsWith(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().StartsWith);
    }

    [TestMethod]
    public void StartsWithCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.StartsWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().StartsWithCi);
    }

    [TestMethod]
    public void DoesNotStartWith()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.DoesNotStartWith(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().DoesNotStartWith);
    }

    [TestMethod]
    public void DoesNotStartWithCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.DoesNotStartWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().DoesNotStartWithCi);
    }

    [TestMethod]
    public void EndsWith()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.EndsWith(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().EndsWith);
    }

    [TestMethod]
    public void EndsWithCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.EndsWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().EndsWithCi);
    }

    [TestMethod]
    public void DoesNotEndWith()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.DoesNotEndWith(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().DoesNotEndWith);
    }

    [TestMethod]
    public void DoesNotEndWithCaseInsensitive()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.DoesNotEndWithCaseInsensitive(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().DoesNotEndWithCi);
    }

    [TestMethod]
    public void Regex()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.Regex(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().Regex);
    }

    [TestMethod]
    public void RegexNoMatches()
    {
        // Arrange
        const string input = "/users";

        // Act
        _builder.RegexNoMatches(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().RegexNoMatches);
    }

    [TestMethod]
    public void MinLength()
    {
        // Arrange
        const int input = 10;

        // Act
        _builder.MinLength(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().MinLength);
    }

    [TestMethod]
    public void MaxLength()
    {
        // Arrange
        const int input = 10;

        // Act
        _builder.MaxLength(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().MaxLength);
    }

    [TestMethod]
    public void ExactLength()
    {
        // Arrange
        const int input = 10;

        // Act
        _builder.ExactLength(input);

        // Assert
        Assert.AreEqual(input, _builder.Build().ExactLength);
    }
}
