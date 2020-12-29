namespace HttPlaceholder.Domain
{
    public static class Constants
    {
        public static string[] InputFileSeparators = {"%%", ","};

        // Line endings
        public const string WindowsLineEndingType = "windows";

        public const string UnixLineEndingType = "unix";

        // Images
        public const string JpegType = "jpeg";
        public const string PngType = "png";
        public const string BmpType = "bmp";
        public const string GifType = "gif";
        public static string[] AllowedImageTypes = {JpegType, PngType, BmpType, GifType};
    }
}
