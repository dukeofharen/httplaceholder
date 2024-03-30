using System;
using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Domain;

/// <summary>
///     A class that contains several constants for use in HttPlaceholder.
/// </summary>
public static class Constants
{
    /// <summary>
    ///     The default scenario state.
    /// </summary>
    public const string DefaultScenarioState = "Start";

    /// <summary>
    ///     The number of seconds a regex can execute before timing out.
    /// </summary>
    public const int RegexTimeoutSeconds = 10;

    /// <summary>
    ///     An array of separator characters for providing multiple input file paths.
    /// </summary>
    public static readonly string[] InputFileSeparators = ["%%", ","];
}

/// <summary>
///     A class that contains several stub types.
/// </summary>
public static class StubTypes
{
    /// <summary>
    ///     The JSON stub type.
    /// </summary>
    public const string StubJsonType = "json";

    /// <summary>
    ///     The YAML stub type.
    /// </summary>
    public const string StubYamlType = "yaml";
}

/// <summary>
///     A class containing file and folder names.
/// </summary>
public static class FileNames
{
    // File storage folder and file names.
    /// <summary>
    ///     The stubs folder name.
    /// </summary>
    public const string StubsFolderName = "stubs";

    /// <summary>
    ///     The requests folder name.
    /// </summary>
    public const string RequestsFolderName = "requests";

    /// <summary>
    ///     The responses folder name.
    /// </summary>
    public const string ResponsesFolderName = "responses";

    /// <summary>
    ///     The scenarios folder name.
    /// </summary>
    public const string ScenariosFolderName = "scenarios";

    /// <summary>
    ///     The metadata file name.
    /// </summary>
    public const string MetadataFileName = "metadata.json";
}

/// <summary>
///     A class containing default configuration values.
/// </summary>
public static class DefaultConfiguration
{
    /// <summary>
    ///     The default HTTP port.
    /// </summary>
    public const int DefaultHttpPort = 5000;

    /// <summary>
    ///     The default HTTPS port.
    /// </summary>
    public const int DefaultHttpsPort = 5050;

    /// <summary>
    ///     The default private key PFX path.
    /// </summary>
    public const string DefaultPfxPath = "key.pfx";

    /// <summary>
    ///     The default private key PFX password.
    /// </summary>
    public const string DefaultPfxPassword = "1234";

    /// <summary>
    ///     Whether the proxy headers should be read by default.
    /// </summary>
    public const bool ReadProxyHeaders = true;

    /// <summary>
    ///     Whether to allow global file search for the file response writer.
    /// </summary>
    public const bool AllowGlobalFileSearch = false;

    /// <summary>
    ///     Whether HTTPS is enabled by default.
    /// </summary>
    public const bool UseHttps = true;

    /// <summary>
    ///     Whether the user interface is enabled by default.
    /// </summary>
    public const bool EnableUserInterface = true;

    /// <summary>
    ///     The default number of requests that should be kept by default.
    /// </summary>
    public const int DefaultOldRequestsQueueLength = 40;

    /// <summary>
    ///     Whether the CleanOldRequests job is enabled by default.
    /// </summary>
    public const bool CleanOldRequestsInBackgroundJob = true;

    /// <summary>
    ///     Whether the responses should be stored by default.
    /// </summary>
    public const bool StoreResponses = false;

    /// <summary>
    ///     The default maximum "extra duration" milliseconds.
    /// </summary>
    public const int DefaultMaximumExtraDuration = 60000;
}

/// <summary>
///     A class containing several mime types.
/// </summary>
public static class MimeTypes
{
    /// <summary>
    ///     The JSON mime type.
    /// </summary>
    public const string JsonMime = "application/json";

    /// <summary>
    ///     The plain text mime type.
    /// </summary>
    public const string TextMime = "text/plain";

    /// <summary>
    ///     The YAML text mime type.
    /// </summary>
    public const string YamlTextMime = "text/yaml";

    /// <summary>
    ///     The YAML application mime type.
    /// </summary>
    public const string YamlApplicationMime = "application/x-yaml";

    /// <summary>
    ///     The HTML mime type.
    /// </summary>
    public const string HtmlMime = "text/html";

    /// <summary>
    ///     The XML text mime type.
    /// </summary>
    public const string XmlTextMime = "text/xml";

    /// <summary>
    ///     The XML application mime type.
    /// </summary>
    public const string XmlApplicationMime = "application/xml";

    /// <summary>
    ///     The URL encoded form mime type.
    /// </summary>
    public const string UrlEncodedFormMime = "application/x-www-form-urlencoded";

    /// <summary>
    ///     The multipart form data mime type.
    /// </summary>
    public const string MultipartFormDataMime = "multipart/form-data";
}

/// <summary>
///     A class containing the HTTP header keys.
/// </summary>
public static class HeaderKeys
{
    /// <summary>
    ///     The content type header key.
    /// </summary>
    public const string ContentType = "Content-Type";

    /// <summary>
    ///     The content length header key.
    /// </summary>
    public const string ContentLength = "Content-Length";

    /// <summary>
    ///     The host header key.
    /// </summary>
    public const string Host = "Host";

    /// <summary>
    ///     The connection header key.
    /// </summary>
    public const string Connection = "Connection";

    /// <summary>
    ///     The accept encoding header key.
    /// </summary>
    public const string AcceptEncoding = "Accept-Encoding";

    /// <summary>
    ///     The transfer encoding header key.
    /// </summary>
    public const string TransferEncoding = "Transfer-Encoding";

    /// <summary>
    ///     The HttPlaceholder correlation header key.
    /// </summary>
    public const string XHttPlaceholderCorrelation = "X-HttPlaceholder-Correlation";

    /// <summary>
    ///     The HttPlaceholder executed stub header key.
    /// </summary>
    public const string XHttPlaceholderExecutedStub = "X-HttPlaceholder-ExecutedStub";

    /// <summary>
    ///     The Postman token header key.
    /// </summary>
    public const string PostmanToken = "Postman-Token";

    /// <summary>
    ///     The location header key.
    /// </summary>
    public const string Location = "Location";

    /// <summary>
    ///     The authorization header key.
    /// </summary>
    public const string Authorization = "Authorization";
}

/// <summary>
///     A static constants class used for defining caching keys.
/// </summary>
public static class CachingKeys
{
    /// <summary>
    ///     The scenario state caching key.
    /// </summary>
    public const string ScenarioState = "scenarioState";
}

/// <summary>
///     A static constants class used for the CLI arguments.
/// </summary>
public static class CliArgs
{
    /// <summary>
    ///     CLI arguments that enable verbose logging.
    /// </summary>
    public static readonly string[] VerboseArgs = ["-V", "--verbose"];

    /// <summary>
    ///     CLI arguments that show the HttPlaceholder version.
    /// </summary>
    public static readonly string[] VersionArgs = ["-v", "--version"];

    /// <summary>
    ///     CLI arguments that show the help page.
    /// </summary>
    public static readonly string[] HelpArgs = ["-h", "--help", "-?"];

    /// <summary>
    ///     Determines whether verbose logging is enabled.
    /// </summary>
    /// <param name="args">The CLI args.</param>
    /// <returns>True if verbose logging is enabled; false otherwise.</returns>
    public static bool IsVerbose(IEnumerable<string> args)
    {
        var env = Environment.GetEnvironmentVariable("verbose");
        return args.Any(VerboseArgs.Contains) ||
               string.Equals(env, "true", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
///     A static class that contains the keys in the stub metadata dictionary.
/// </summary>
public static class StubMetadataKeys
{
    /// <summary>
    ///     Used to store the stub filename (if present).
    /// </summary>
    public const string Filename = "filename";
}
