using ImageMagick;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A static class with utilities for working with Magick.NET.
/// </summary>
public static class ImageMagickUtilities
{
    /// <summary>
    ///     Inverts the given RGB color.
    /// </summary>
    /// <param name="color">The input color.</param>
    /// <returns>The inverted color.</returns>
    public static MagickColor InvertRgbColor(this MagickColor color) =>
        new(
            (byte)(255 - color.R),
            (byte)(255 - color.G),
            (byte)(255 - color.B),
            color.A);
}
