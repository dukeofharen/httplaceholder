namespace HttPlaceholder.Domain.Enums
{
    /// <summary>
    /// An enum for specifying the type of image to return for the stub image.
    /// </summary>
    public enum ResponseImageType
    {
        /// <summary>
        /// Image type not set.
        /// </summary>
        NotSet,

        /// <summary>
        /// Create a JPEG.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Create a BMP.
        /// </summary>
        Bmp,

        /// <summary>
        /// Create a PNG.
        /// </summary>
        Png,

        /// <summary>
        /// Create a GIF.
        /// </summary>
        Gif
    }
}
