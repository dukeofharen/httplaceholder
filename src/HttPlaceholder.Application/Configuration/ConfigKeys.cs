using System.Collections.Generic;
using HttPlaceholder.Domain.Enums;
using static HttPlaceholder.Application.Configuration.ConfigMetadataModel;

namespace HttPlaceholder.Application.Configuration;

/// <summary>
///     A class that contains constants for every possible configuration option.
/// </summary>
public static class ConfigKeys
{
    /// <summary>
    ///     Constant for apiUsername.
    /// </summary>
    public const string ApiUsernameKey = "apiUsername";

    /// <summary>
    ///     Constant for apiPassword.
    /// </summary>
    public const string ApiPasswordKey = "apiPassword";

    /// <summary>
    ///     Constant for httpsPort.
    /// </summary>
    public const string HttpsPortKey = "httpsPort";

    /// <summary>
    ///     Constant for inputFile.
    /// </summary>
    public const string InputFileKey = "inputFile";

    /// <summary>
    ///     Constant for oldRequestsQueueLength.
    /// </summary>
    public const string OldRequestsQueueLengthKey = "oldRequestsQueueLength";

    /// <summary>
    ///     Constant for storeResponses.
    /// </summary>
    public const string StoreResponses = "storeResponses";

    /// <summary>
    ///     Constant for cleanOldRequestsInBackgroundJob.
    /// </summary>
    public const string CleanOldRequestsInBackgroundJob = "cleanOldRequestsInBackgroundJob";

    /// <summary>
    ///     Constant for pfxPassword.
    /// </summary>
    public const string PfxPasswordKey = "pfxPassword";

    /// <summary>
    ///     Constant for readProxyHeaders.
    /// </summary>
    public const string ReadProxyHeaders = "readProxyHeaders";

    /// <summary>
    ///     Constant for safeProxyIps.
    /// </summary>
    public const string SafeProxyIps = "safeProxyIps";

    /// <summary>
    ///     Constant for pfxPath.
    /// </summary>
    public const string PfxPathKey = "pfxPath";

    /// <summary>
    ///     Constant for port.
    /// </summary>
    public const string PortKey = "port";

    /// <summary>
    ///     Constant for useHttps.
    /// </summary>
    public const string UseHttpsKey = "useHttps";

    /// <summary>
    ///     Constant for enableRequestLogging.
    /// </summary>
    public const string EnableRequestLoggingKey = "enableRequestLogging";

    /// <summary>
    ///     Constant for fileStorageLocation.
    /// </summary>
    public const string FileStorageLocationKey = "fileStorageLocation";

    /// <summary>
    ///     Constant for useInMemoryStorage.
    /// </summary>
    public const string UseInMemoryStorage = "useInMemoryStorage";

    /// <summary>
    ///     Constant for mysqlConnectionString.
    /// </summary>
    public const string MysqlConnectionStringKey = "mysqlConnectionString";

    /// <summary>
    ///     Constant for sqliteConnectionString.
    /// </summary>
    public const string SqliteConnectionStringKey = "sqliteConnectionString";

    /// <summary>
    ///     Constant for apiUsername.
    /// </summary>
    public const string SqlServerConnectionStringKey = "sqlServerConnectionString";

    /// <summary>
    ///     Constant for enableUserInterface.
    /// </summary>
    public const string EnableUserInterface = "enableUserInterface";

    /// <summary>
    ///     Constant for maximumExtraDurationMillisKey.
    /// </summary>
    public const string MaximumExtraDurationMillisKey = "maximumExtraDurationMillisKey";

    /// <summary>
    ///     Constant for healthcheckOnRootUrl.
    /// </summary>
    public const string HealthcheckOnRootUrl = "healthcheckOnRootUrl";

    /// <summary>
    ///     Constant for configJsonLocation.
    /// </summary>
    public const string ConfigJsonLocationKey = "configJsonLocation";

    private static readonly IDictionary<ConfigKeyType, string> _configKeyTypes = new Dictionary<ConfigKeyType, string>
    {
        {ConfigKeyType.Authentication, "Authentication"},
        {ConfigKeyType.Web, "Web"},
        {ConfigKeyType.Storage, "Storage"},
        {ConfigKeyType.Gui, "GUI"},
        {ConfigKeyType.Stub, "Stub"},
        {ConfigKeyType.Configuration, "Configuration"}
    };

    private static readonly ConfigMetadataModel[] _configMetadata =
    {
        Create(
            ApiUsernameKey,
            "the username for securing the REST API",
            "user",
            "Authentication:ApiUsername",
            ConfigKeyType.Authentication,
            null,
            null,
            null),
        Create(
            ApiPasswordKey,
            "the password for securing the REST API",
            "pass",
            "Authentication:ApiPassword",
            ConfigKeyType.Authentication,
            null,
            true,
            null),
        Create(
            HttpsPortKey,
            "the port HttPlaceholder should run under when HTTPS is enabled. Listen on multiple ports by separating ports with comma.",
            "5050",
            "Web:HttpsPort",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            InputFileKey,
            "for input file, you can both provide a path to a .yml file (to load only that file) or provide a path to a folder containing .yml files (which will all be loaded in that case).",
            @"C:\path\to\stubsfolder or C:\path\to\stubsfolder,C:\path\to\file.yml for multiple paths",
            "Storage:InputFile",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            OldRequestsQueueLengthKey,
            "the maximum number of HTTP requests that the in memory stub source (used for the REST API) should store before truncating old records. Default: 40.",
            "100",
            "Storage:OldRequestsQueueLength",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            StoreResponses,
            "whether the responses as returned by HttPlaceholder should be stored. Default: false",
            "true",
            "Storage:StoreResponses",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            CleanOldRequestsInBackgroundJob,
            "whether the cleaning of old requests should be done in a background job. If set to true, will delete old requests in a background job that runs once in 5 minutes. If set to false, will clean old requests every time a request is handled. Default: true.",
            "true",
            "Storage:CleanOldRequestsInBackgroundJob",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            PfxPasswordKey,
            "the password for the .pfx file which should be used in the case HTTPS is enabled",
            "112233",
            "Web:PfxPassword",
            ConfigKeyType.Web,
            null,
            true,
            null),
        Create(
            ReadProxyHeaders,
            "whether the proxy headers 'X-Forwarded-For', 'X-Forwarded-Host' and 'X-Forwarded-Proto' should be taken into account when determining the IP, hostname and protocol",
            "true",
            "Web:ReadProxyHeaders",
            ConfigKeyType.Web,
            true,
            null,
            null),
        Create(
            SafeProxyIps,
            "the proxy IPs which are considered safe when reading the 'X-Forwarded-For', 'X-Forwarded-Host' and 'X-Forwarded-Proto' headers. Localhost is always permitted. Separate multiple IPs by using a comma.",
            "1.1.1.1,2.2.2.2",
            "Web:SafeProxyIps",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            PfxPathKey,
            "the path to the .pfx file in the case HTTPS is enabled. When no path is provided, the default .pfx file is used",
            @"C:\path\to\privatekey.pfx",
            "Web:PfxPath",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            PortKey,
            "the HTTP port HttPlaceholder should run under. Listen on multiple ports by separating ports with comma.",
            "5000",
            "Web:HttpPort",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            UseHttpsKey,
            "whether HTTPS should be enabled or not. Default: false",
            "true",
            "Web:UseHttps",
            ConfigKeyType.Web,
            true,
            null,
            null),
        Create(
            EnableRequestLoggingKey,
            "whether stub requests should be logged to the terminal window. Default: false",
            "false",
            "Storage:EnableRequestLogging",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            FileStorageLocationKey,
            "a location where the stubs and requests are stored. This is enabled by default and, if you omit this property, the stubs will be saved in your user profile folder. Specify 'useInMemoryStorage' to only save stubs in memory.",
            @"C:\httplaceholder_storage",
            "Storage:FileStorageLocation",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            UseInMemoryStorage,
            "whether stubs should only be saved in memory. The stubs and requests will be deleted after restarting the application. Default: false",
            "false",
            "Storage:UseInMemoryStorage",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            MysqlConnectionStringKey,
            "a connection string that needs to be filled in if you want to use MySQL",
            "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder;Allow User Variables=true",
            "ConnectionStrings:MySql",
            ConfigKeyType.Storage,
            null,
            true,
            null),
        Create(
            SqliteConnectionStringKey,
            "a connection string that needs to be filled in if you want to use SQLite",
            @"Data Source=C:\tmp\httplaceholder.db",
            "ConnectionStrings:Sqlite",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            SqlServerConnectionStringKey,
            "a connection string that needs to be filled in if you want to use MSSQL",
            "Server=localhost,2433;Database=httplaceholder;User Id=sa;Password=Password123",
            "ConnectionStrings:SqlServer",
            ConfigKeyType.Storage,
            null,
            true,
            null),
        Create(
            EnableUserInterface,
            "whether the user interface should be enabled or not. The user interface is, if enabled, located at http://localhost:PORT/ph-ui.",
            "true",
            "Gui:EnableUserInterface",
            ConfigKeyType.Gui,
            true,
            null,
            null),
        Create(
            MaximumExtraDurationMillisKey,
            "the number of milliseconds the 'extraDuration' response writer can make a request wait at most. Defaults to 1 minute (60.000 millis).",
            "60000",
            "Stub:MaximumExtraDurationMillis",
            ConfigKeyType.Stub,
            null,
            null,
            null),
        Create(
            HealthcheckOnRootUrl,
            @"whether the root URL of HttPlaceholder (so ""/"") can be configured as stub or always returns 200 OK. Defaults to false.",
            "true",
            "Stub:HealthcheckOnRootUrl",
            ConfigKeyType.Stub,
            true,
            null,
            null),
        Create(
            ConfigJsonLocationKey,
            "the location of the config.json file. This JSON file contains all possible configuration settings and a default value per setting. You can copy this file to any location on your PC. Don't put the config file in the installation folder, because these files will be overwritten when an update is installed.",
            @"F:\httplaceholder\config.json",
            null,
            ConfigKeyType.Configuration,
            null,
            null,
            null)
    };

    /// <summary>
    ///     Returns a dictionary of <see cref="ConfigKeyType" /> and translation combinations.
    /// </summary>
    /// <returns>A dictionary of <see cref="ConfigKeyType" /> and translation combinations.</returns>
    public static IDictionary<ConfigKeyType, string> GetConfigKeyTypes() => _configKeyTypes;


    /// <summary>
    ///     A method that returns all configuration metadata.
    /// </summary>
    /// <returns>The configuration metadata.</returns>
    public static IEnumerable<ConfigMetadataModel> GetConfigMetadata() => _configMetadata;
}
