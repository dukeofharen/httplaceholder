namespace HttPlaceholder.Domain
{
    public static class Constants
    {
        public static string[] InputFileSeparators = {"%%", ","};

        // File storage folder and file names.
        public const string StubsFolderName = "stubs";
        public const string RequestsFolderName = "requests";
        public const string MetadataFileName = "metadata.json";

        // Default config values
        public const int DefaultHttpPort = 5000;
        public const int DefaultHttpsPort = 5050;
        public const string DefaultPfxPath = "key.pfx";
        public const string DefaultPfxPassword = "1234";
        public const bool UseHttps = true;
        public const bool EnableUserInterface = true;
        public const bool UseNewUi = false;
        public const int DefaultOldRequestsQueueLength = 40;
        public const int DefaultMaximumExtraDuration = 60000;
    }
}
