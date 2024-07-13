using HttPlaceholder.Common.Utilities;
using ImageMagick;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class ImageMagickUtilitiesFacts
{
    [TestMethod]
    public void InvertRgbColor_HappyFlow()
    {
        // Arrange
        var inputColor = new MagickColor("#24ed01");

        // Act
        var result = inputColor.InvertRgbColor();

        // Assert
        Assert.AreEqual(255, result.A);
        Assert.AreEqual(219, result.R);
        Assert.AreEqual(18, result.G);
        Assert.AreEqual(254, result.B);
    }
}
