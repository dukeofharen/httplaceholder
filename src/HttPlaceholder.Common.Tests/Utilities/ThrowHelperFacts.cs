using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class ThrowHelperFacts
{
    [TestMethod]
    public void ThrowIfNull_Null_NoMessage_ShouldThrow()
    {
        // Arrange
        var message = string.Empty;

        // Act
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            ThrowHelper.ThrowIfNull<string, InvalidOperationException>(null, message));

        // Assert
        Assert.AreEqual("Operation is not valid due to the current state of the object.", exception.Message);
    }

    [TestMethod]
    public void ThrowIfNull_Null_WithMessage_ShouldThrow()
    {
        // Arrange
        const string message = "Something went wrong.";

        // Act
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            ThrowHelper.ThrowIfNull<string, InvalidOperationException>(null, message));

        // Assert
        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod]
    public void ThrowIfNull_NotNull_HappyFlow()
    {
        // Act
        var result = ThrowHelper.ThrowIfNull<string, InvalidOperationException>("input");

        // Assert
        Assert.AreEqual("input", result);
    }
}
