using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class ObjectUtilitiesFacts
{
    [TestMethod]
    public async Task IfNullAsync_ResultIsNotNull()
    {
        // Arrange
        var task = "test123".AsTask();

        // Act
        var result = await task.IfNullAsync(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual("test123", result);
    }

    [TestMethod]
    public async Task IfNullAsync_ResultIsNull()
    {
        // Arrange
        var task = ((string)null).AsTask();

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            task.IfNullAsync(() => throw new InvalidOperationException()));
    }

    [TestMethod]
    public void IfNull_Value_ResultIsNotNull()
    {
        // Arrange
        const string value = "test123";

        // Act
        var result = value.IfNull(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void IfNull_Value_ResultIsNull()
    {
        // Arrange
        string value = null;

        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            value.IfNull(() => throw new InvalidOperationException()));
    }

    [TestMethod]
    public async Task IfAsync_FuncReturnsTrue()
    {
        // Arrange
        var input = "some input".AsTask();

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            input.IfAsync(i => !string.IsNullOrWhiteSpace(i), _ => throw new InvalidOperationException()));
    }

    [TestMethod]
    public async Task IfAsync_FuncReturnsFalse()
    {
        // Arrange
        var input = "some input".AsTask();

        // Act
        var result = await input.IfAsync(string.IsNullOrWhiteSpace, _ => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(await input, result);
    }

    [TestMethod]
    public void If_FuncReturnsTrue()
    {
        // Arrange
        const string input = "some input";

        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            input.If(i => !string.IsNullOrWhiteSpace(i), _ => throw new InvalidOperationException()));
    }

    [TestMethod]
    public void If_FuncReturnsFalse()
    {
        // Arrange
        const string input = "some input";

        // Act
        var result = input.If(string.IsNullOrWhiteSpace, _ => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public async Task MapAsync_HappyFlow()
    {
        // Arrange
        var input = "abc123".AsTask();

        // Act
        var result = await input.MapAsync(r => r.Length);

        // Assert
        Assert.AreEqual(6, result);
    }

    [TestMethod]
    public void Map_HappyFlow()
    {
        // Arrange
        const string input = "abc123";

        // Act
        var result = input.Map(r => r.Length);

        // Assert
        Assert.AreEqual(6, result);
    }

    [TestMethod]
    public async Task AsTask_HappyFlow()
    {
        // Arrange
        const string input = "abc123";

        // Act
        var result = await input.AsTask();

        // Assert
        Assert.AreEqual(input, result);
    }
}
