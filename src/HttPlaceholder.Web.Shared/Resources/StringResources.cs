namespace HttPlaceholder.Web.Shared.Resources;

/// <summary>
///     A static class that contains several string resources.
/// </summary>
public static class StringResources
{
    /// <summary>
    ///     The command line example.
    /// </summary>
    public const string CmdExample = @"Example: httplaceholder --apiusername user --apipassword pass";

    /// <summary>
    ///     The exception thrown format string.
    /// </summary>
    public const string ExceptionThrown = @"Unexpected exception thrown: {0}";

    /// <summary>
    ///     The explanation header.
    /// </summary>
    public const string ExplanationHeader= @"Run this application with argument '-h' or '--help' to get more info about the command line arguments.
When running in to trouble, or just see what's going on, run this application with argument '-V' or '--verbose' to print the configuration variables.
You can also set the ""verbose"" environment variable (without quotes) to ""true"" to enable verbose logging.
    Visit https://httplaceholder.org/";

    /// <summary>
    ///     The version header.
    /// </summary>
    public const string VersionHeader = @"HttPlaceholder {0} - (c) {1} Ducode";
}
