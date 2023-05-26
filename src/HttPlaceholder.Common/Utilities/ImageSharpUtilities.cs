using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class for working with ImageSharp.
/// </summary>
public static class ImageSharpUtilities
{
    /// <summary>
    ///     Inverts a given color.
    /// </summary>
    /// <param name="input">The color.</param>
    /// <returns>The inverted <see cref="Color" />.</returns>
    public static Color InvertColor(this Color input)
    {
        var pixel = input.ToPixel<Rgba32>();
        return Color.FromRgb((byte)(255 - pixel.R), (byte)(255 - pixel.G), (byte)(255 - pixel.B));
    }

    /// <summary>
    ///     Applies a watermark to a given image.
    /// </summary>
    /// <param name="processingContext">The image processing context.</param>
    /// <param name="font">The font the watermark should be in.</param>
    /// <param name="text">The watermark text.</param>
    /// <param name="color">The HTML encoded color (e.g. '#123456').</param>
    /// <param name="padding">The padding in pixels.</param>
    /// <param name="wordwrap">True if the words should be wrapped.</param>
    /// <returns>The input <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext ApplyScalingWaterMark(this IImageProcessingContext processingContext,
        Font font,
        string text,
        Color color,
        float padding,
        bool wordwrap) =>
        wordwrap
            ? processingContext.ApplyScalingWaterMarkWordWrap(font, text, color, padding)
            : processingContext.ApplyScalingWaterMarkSimple(font, text, color);

    private static IImageProcessingContext ApplyScalingWaterMarkSimple(
        this IImageProcessingContext processingContext,
        Font font,
        string text,
        Color color)
    {
        var (width, height) = processingContext.GetCurrentSize();

        // measure the text size
        var size = TextMeasurer.Measure(text, new TextOptions(font));

        //find out how much we need to scale the text to fill the space (up or down)
        var scalingFactor = Math.Min(width / size.Width, height / size.Height);

        //create a new font
        var scaledFont = new Font(font, scalingFactor * font.Size);

        var center = new PointF(width / 2, height / 2);
        var textOptions = new TextOptions(scaledFont)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = center
        };
        return processingContext.DrawText(textOptions, text, color);
    }

    private static IImageProcessingContext ApplyScalingWaterMarkWordWrap(
        this IImageProcessingContext processingContext,
        Font font,
        string text,
        Color color,
        float padding)
    {
        var (width, height) = processingContext.GetCurrentSize();
        var targetWidth = width - (padding * 2);
        var targetHeight = height - (padding * 2);

        var targetMinHeight =
            height - (padding * 3); // must be with in a margin width of the target height

        // now we are working i 2 dimensions at once and can't just scale because it will cause the text to
        // reflow we need to just try multiple times

        var scaledFont = font;
        var s = new FontRectangle(0, 0, float.MaxValue, float.MaxValue);

        var scaleFactor = scaledFont.Size / 2; // every time we change direction we half this size
        var trapCount = (int)scaledFont.Size * 2;
        if (trapCount < 10)
        {
            trapCount = 10;
        }

        var isTooSmall = false;

        while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0)
        {
            if (s.Height > targetHeight)
            {
                if (isTooSmall)
                {
                    scaleFactor /= 2;
                }

                scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
                isTooSmall = false;
            }

            if (s.Height < targetMinHeight)
            {
                if (!isTooSmall)
                {
                    scaleFactor /= 2;
                }

                scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
                isTooSmall = true;
            }

            trapCount--;

            s = TextMeasurer.Measure(text, new TextOptions(scaledFont) {WrappingLength = targetWidth});
        }

        var center = new PointF(padding, height / 2);
        var textOptions = new TextOptions(scaledFont)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            WrappingLength = targetWidth,
            Origin = center
        };
        return processingContext.DrawText(textOptions, text, color);
    }
}
