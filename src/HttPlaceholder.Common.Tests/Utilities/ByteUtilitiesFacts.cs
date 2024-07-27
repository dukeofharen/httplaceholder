using System.Collections.Generic;
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
        const string input = "abc123";

        // Act
        var result = Encoding.UTF8.GetBytes(input).IsValidAscii();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValidAscii_OnlyPartiallyAscii_ShouldReturnFalse()
    {
        // Arrange
        var input = new byte[] { 1, 2, 3, 200, 201 };

        // Act
        var result = input.IsValidAscii();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValidAscii_NoneAscii_ShouldReturnFalse()
    {
        // Arrange
        var input = new byte[] { 200, 201, 203, 204 };

        // Act
        var result = input.IsValidAscii();

        // Assert
        Assert.IsFalse(result);
    }

    public static IEnumerable<object> ProvideIsValidTextData =>
        new[]
        {
            new object[] { new byte[] { 1, 2, 3 }, "application/pdf", true },
            new object[] { new byte[] { 1, 2, 3, 128, 129 }, "application/pdf", false },
            new object[] { new byte[] { 128, 129, 130 }, "application/pdf", false },
            new object[] { new byte[] { 128, 129, 130 }, "text/plain", true },
            new object[] { new byte[] { 1, 2, 3 }, "text/csv", true },
            new object[] { new byte[] { 1, 2, 3 }, "", true },
            new object[] { new byte[] { 1, 2, 3 }, null, true },
            new object[] { new byte[] { 128, 129, 130 }, "", false },
            new object[] { new byte[] { 128, 129, 130 }, null, false }
        };

    [TestMethod]
    [DynamicData(nameof(ProvideIsValidTextData))]
    public void IsValidText_HappyFlow(byte[] bytes, string contentType, bool expectedResult)
    {
        // Act
        var result = bytes.IsValidText(contentType);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
