using System.Text;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class ByteUtilitiesFacts
{
    [TestMethod]
    public void IsValidAscii_InputIsNull_ShouldReturnTrue()
    {
        // Act
        var result = ByteUtilities.IsValidAscii(null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValidAscii_EverythingAscii_ShouldReturnTrue()
    {
        // Arrange
        var input = "abc123";

        // Act
        var result = Encoding.UTF8.GetBytes(input).IsValidAscii();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValidAscii_OnlyPartiallyAscii_ShouldReturnFalse()
    {
        // Arrange
        var input = new byte[] {1, 2, 3, 200, 201};

        // Act
        var result = input.IsValidAscii();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValidAscii_NoneAscii_ShouldReturnFalse()
    {
        // Arrange
        var input = new byte[] {200, 201, 203, 204};

        // Act
        var result = input.IsValidAscii();

        // Assert
        Assert.IsFalse(result);
    }
}
