using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class ObjectUtilitiesFacts
{
    [TestMethod]
    public async Task IfNullAsync_ResultIsNotNull()
    {
        // Arrange
        var task = Task.FromResult("test123");

        // Act
        var result = await task.IfNullAsync(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual("test123", result);
    }

    [TestMethod]
    public async Task IfNullAsync_ResultIsNull()
    {
        // Arrange
        var task = Task.FromResult((string)null);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            task.IfNullAsync(() => throw new InvalidOperationException()));
    }

    [TestMethod]
    public void IfNull_Value_ResultIsNotNull()
    {
        // Arrange
        var value = "test123";

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
        var input = Task.FromResult("some input");

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            input.IfAsync(i => !string.IsNullOrWhiteSpace(i), i => throw new InvalidOperationException()));
    }

    [TestMethod]
    public async Task IfAsync_FuncReturnsFalse()
    {
        // Arrange
        var input = Task.FromResult("some input");

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
            input.If(i => !string.IsNullOrWhiteSpace(i), i => throw new InvalidOperationException()));
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
    public async Task IfTrueAsync_True()
    {
        // Arrange
        var input = Task.FromResult(true);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            input.IfTrueAsync(() => throw new InvalidOperationException()));
    }

    [TestMethod] public async Task IfTrueAsync_False()
    {
        // Arrange
        var input = Task.FromResult(false);

        // Act
        var result = await input.IfTrueAsync(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(await input, result);
    }

    [TestMethod]
    public void IfTrue_True()
    {
        // Arrange
        const bool input = true;

        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            input.IfTrue(() => throw new InvalidOperationException()));
    }

    [TestMethod] public void IfTrue_False()
    {
        // Arrange
        const bool input = false;

        // Act
        var result = input.IfTrue(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public async Task IfFalseAsync_False()
    {
        // Arrange
        var input = Task.FromResult(false);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            input.IfFalseAsync(() => throw new InvalidOperationException()));
    }

    [TestMethod] public async Task IfFalseAsync_True()
    {
        // Arrange
        var input = Task.FromResult(true);

        // Act
        var result = await input.IfFalseAsync(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(await input, result);
    }

    [TestMethod]
    public void IfFalse_False()
    {
        // Arrange
        const bool input = false;

        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            input.IfFalse(() => throw new InvalidOperationException()));
    }

    [TestMethod] public void IfFalse_True()
    {
        // Arrange
        const bool input = true;

        // Act
        var result = input.IfFalse(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public async Task MapAsync_HappyFlow()
    {
        // Arrange
        var input = Task.FromResult("abc123");

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
}
