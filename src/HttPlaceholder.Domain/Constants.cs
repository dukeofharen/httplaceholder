namespace HttPlaceholder.Domain
{
    public static class Constants
    {
        public static string[] InputFileSeparators = {"%%", ","};

        // Images
        public const string JpegType = "jpeg";
        public const string PngType = "png";
        public const string BmpType = "bmp";
        public const string GifType = "gif";
        public static string[] AllowedImageTypes = {JpegType, PngType, BmpType, GifType};

        // File storage folder and file names.
        public const string StubsFolderName = "stubs";
        public const string RequestsFolderName = "requests";
        public const string MetadataFileName = "metadata.json";
    }
}
