using HttPlaceholder.Client.Dto.Enums;

namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
/// A model for storing settings about the stub image that should be generated.
/// </summary>
public class StubResponseImageDto
{
    /// <summary>
    /// Gets or sets the image type. Possibilities: jpeg, png, bmp and gif.
    /// </summary>
    public ResponseImageType? Type { get; set; }

    /// <summary>
    /// Gets or sets the image width in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the image height in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the background color in HEX.
    /// </summary>
    public string BackgroundColor { get; set; } = "#3d3d3d";

    /// <summary>
    /// Gets or sets the text that should be drawn in the image.
    /// </summary>
    public string Text { get; set; } = "HttPlaceholder";

    /// <summary>
    /// Gets or sets the fontsize for the text in the image.
    /// </summary>
    public int FontSize { get; set; } = 7;

    /// <summary>
    /// Gets or sets the font color.
    /// </summary>
    public string FontColor { get; set; }

    /// <summary>
    /// Gets or sets the image quality in the case of JPEG image.
    /// </summary>
    public int JpegQuality { get; set; } = 95;

    /// <summary>
    /// Gets or sets whether the text should be wrapped across the image or not.
    /// </summary>
    public bool WordWrap { get; set; }
}