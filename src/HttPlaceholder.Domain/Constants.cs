namespace HttPlaceholder.Domain;

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
    public const int DefaultOldRequestsQueueLength = 40;
    public const int DefaultMaximumExtraDuration = 60000;

    // Scenario values.
    public const string DefaultScenarioState = "Start";

    // Mime types.
    public const string JsonMime = "application/json";
    public const string TextMime = "text/plain";
    public const string YamlTextMime = "text/yaml";
    public const string YamlApplicationMime = "application/x-yaml";
    public const string HtmlMime = "text/html";
    public const string XmlTextMime = "text/xml";
    public const string XmlApplicationMime = "application/xml";
    public const string UrlEncodedFormMime = "application/x-www-form-urlencoded";
    public const string MultipartFormDataMime = "multipart/form-data";
}
