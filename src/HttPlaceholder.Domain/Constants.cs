namespace HttPlaceholder.Domain;

/// <summary>
/// A class that contains several constants for use in HttPlaceholder.
/// </summary>
public static class Constants
{
    /// <summary>
    /// An array of separator characters for providing multiple input file paths.
    /// </summary>
    public static string[] InputFileSeparators = {"%%", ","};

    // File storage folder and file names.
    /// <summary>
    /// The stubs folder name.
    /// </summary>
    public const string StubsFolderName = "stubs";

    /// <summary>
    /// The requests folder name.
    /// </summary>
    public const string RequestsFolderName = "requests";

    /// <summary>
    /// The metadata file name.
    /// </summary>
    public const string MetadataFileName = "metadata.json";

    // Default config values
    /// <summary>
    /// The default HTTP port.
    /// </summary>
    public const int DefaultHttpPort = 5000;

    /// <summary>
    /// The default HTTPS port.
    /// </summary>
    public const int DefaultHttpsPort = 5050;

    /// <summary>
    /// The default private key PFX path.
    /// </summary>
    public const string DefaultPfxPath = "key.pfx";

    /// <summary>
    /// The default private key PFX password.
    /// </summary>
    public const string DefaultPfxPassword = "1234";

    /// <summary>
    /// Whether HTTPS is enabled by default.
    /// </summary>
    public const bool UseHttps = true;

    /// <summary>
    /// Whether the user interface is enabled by default.
    /// </summary>
    public const bool EnableUserInterface = true;

    /// <summary>
    /// The default number of requests that should be kept by default.
    /// </summary>
    public const int DefaultOldRequestsQueueLength = 40;

    /// <summary>
    /// The default maximum "extra duration" milliseconds.
    /// </summary>
    public const int DefaultMaximumExtraDuration = 60000;

    // Scenario values.
    /// <summary>
    /// The default scenario state.
    /// </summary>
    public const string DefaultScenarioState = "Start";

    // Mime types.
    /// <summary>
    /// The JSON mime type.
    /// </summary>
    public const string JsonMime = "application/json";

    /// <summary>
    /// The plain text mime type.
    /// </summary>
    public const string TextMime = "text/plain";

    /// <summary>
    /// The YAML text mime type.
    /// </summary>
    public const string YamlTextMime = "text/yaml";

    /// <summary>
    /// The YAML application mime type.
    /// </summary>
    public const string YamlApplicationMime = "application/x-yaml";

    /// <summary>
    /// The HTML mime type.
    /// </summary>
    public const string HtmlMime = "text/html";

    /// <summary>
    /// The XML text mime type.
    /// </summary>
    public const string XmlTextMime = "text/xml";

    /// <summary>
    /// The XML application mime type.
    /// </summary>
    public const string XmlApplicationMime = "application/xml";

    /// <summary>
    /// The URL encoded form mime type.
    /// </summary>
    public const string UrlEncodedFormMime = "application/x-www-form-urlencoded";

    /// <summary>
    /// The multipart form data mime type.
    /// </summary>
    public const string MultipartFormDataMime = "multipart/form-data";
}
