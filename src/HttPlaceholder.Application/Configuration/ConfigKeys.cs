using System.Collections.Generic;
using HttPlaceholder.Application.Configuration.Attributes;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Configuration;

/// <summary>
/// A class that contains constants for every possible configuration option.
/// </summary>
public static class ConfigKeys
{
    /// <summary>
    /// Constant for apiUsername.
    /// </summary>
    [ConfigKey(
        Description = "the username for securing the REST API",
        Example = "user",
        ConfigPath = "Authentication:ApiUsername",
        ConfigKeyType = ConfigKeyType.Authentication)]
    public const string ApiUsernameKey = "apiUsername";

    /// <summary>
    /// Constant for apiPassword.
    /// </summary>
    [ConfigKey(
        Description = "the password for securing the REST API",
        Example = "pass",
        ConfigPath = "Authentication:ApiPassword",
        ConfigKeyType = ConfigKeyType.Authentication)]
    public const string ApiPasswordKey = "apiPassword";

    /// <summary>
    /// Constant for httpsPort.
    /// </summary>
    [ConfigKey(
        Description =
            "the port HttPlaceholder should run under when HTTPS is enabled. Listen on multiple ports by separating ports with comma.",
        Example = "5050",
        ConfigPath = "Web:HttpsPort",
        ConfigKeyType = ConfigKeyType.Web)]
    public const string HttpsPortKey = "httpsPort";

    /// <summary>
    /// Constant for inputFile.
    /// </summary>
    [ConfigKey(
        Description =
            "for input file, you can both provide a path to a .yml file (to load only that file) or provide a path to a folder containing .yml files (which will all be loaded in that case).",
        Example = @"C:\path\to\stubsfolder or C:\path\to\stubsfolder,C:\path\to\file.yml for multiple paths",
        ConfigPath = "Storage:InputFile",
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string InputFileKey = "inputFile";

    /// <summary>
    /// Constant for oldRequestsQueueLength.
    /// </summary>
    [ConfigKey(
        Description =
            "the maximum number of HTTP requests that the in memory stub source (used for the REST API) should store before truncating old records. Default: 40.",
        Example = "100",
        ConfigPath = "Storage:OldRequestsQueueLength",
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string OldRequestsQueueLengthKey = "oldRequestsQueueLength";

    /// <summary>
    /// Constant for storeResponses.
    /// </summary>
    [ConfigKey(
        Description = "whether the responses as returned by HttPlaceholder should be stored. Default: false",
        Example = "true",
        ConfigPath = "Storage:StoreResponses",
        IsBoolValue = true,
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string StoreResponses = "storeResponses";

    /// <summary>
    /// Constant for cleanOldRequestsInBackgroundJob.
    /// </summary>
    [ConfigKey(
        Description =
            "whether the cleaning of old requests should be done in a background job. If set to true, will delete old requests in a background job that runs once in 5 minutes. If set to false, will clean old requests every time a request is handled. Default: true.",
        Example = "true",
        IsBoolValue = true,
        ConfigPath = "Storage:CleanOldRequestsInBackgroundJob",
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string CleanOldRequestsInBackgroundJob = "cleanOldRequestsInBackgroundJob";

    /// <summary>
    /// Constant for pfxPassword.
    /// </summary>
    [ConfigKey(
        Description = "the password for the .pfx file which should be used in the case HTTPS is enabled",
        Example = "112233",
        ConfigPath = "Web:PfxPassword",
        ConfigKeyType = ConfigKeyType.Web)]
    public const string PfxPasswordKey = "pfxPassword";

    /// <summary>
    /// Constant for readProxyHeaders.
    /// </summary>
    [ConfigKey(
        Description =
            "whether the proxy headers 'X-Forwarded-For', 'X-Forwarded-Host' and 'X-Forwarded-Proto' should be taken into account when determining the IP, hostname and protocol",
        Example = "true",
        IsBoolValue = true,
        ConfigPath = "Web:ReadProxyHeaders",
        ConfigKeyType = ConfigKeyType.Web)]
    public const string ReadProxyHeaders = "readProxyHeaders";

    /// <summary>
    /// Constant for safeProxyIps.
    /// </summary>
    [ConfigKey(
        Description =
            "the proxy IPs which are considered safe when reading the 'X-Forwarded-For', 'X-Forwarded-Host' and 'X-Forwarded-Proto' headers. Localhost is always permitted. Separate multiple IPs by using a comma.",
        Example = "1.1.1.1,2.2.2.2",
        ConfigPath = "Web:SafeProxyIps",
        ConfigKeyType = ConfigKeyType.Web)]
    public const string SafeProxyIps = "safeProxyIps";

    /// <summary>
    /// Constant for pfxPath.
    /// </summary>
    [ConfigKey(
        Description =
            "the path to the .pfx file in the case HTTPS is enabled. When no path is provided, the default .pfx file is used",
        Example = @"C:\path\to\privatekey.pfx",
        ConfigPath = "Web:PfxPath",
        ConfigKeyType = ConfigKeyType.Web)]
    public const string PfxPathKey = "pfxPath";

    /// <summary>
    /// Constant for port.
    /// </summary>
    [ConfigKey(
        Description =
            "the HTTP port HttPlaceholder should run under. Listen on multiple ports by separating ports with comma.",
        Example = "5000",
        ConfigPath = "Web:HttpPort",
        ConfigKeyType = ConfigKeyType.Web)]
    public const string PortKey = "port";

    /// <summary>
    /// Constant for useHttps.
    /// </summary>
    [ConfigKey(
        Description = "whether HTTPS should be enabled or not. Default: false",
        Example = "true",
        ConfigPath = "Web:UseHttps",
        IsBoolValue = true,
        ConfigKeyType = ConfigKeyType.Web)]
    public const string UseHttpsKey = "useHttps";

    /// <summary>
    /// Constant for enableRequestLogging.
    /// </summary>
    [ConfigKey(
        Description = "whether stub requests should be logged to the terminal window. Default: false",
        Example = "false",
        ConfigPath = "Storage:EnableRequestLogging",
        IsBoolValue = true,
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string EnableRequestLoggingKey = "enableRequestLogging";

    /// <summary>
    /// Constant for fileStorageLocation.
    /// </summary>
    [ConfigKey(
        Description =
            "a location where the stubs and requests are stored. This is enabled by default and, if you omit this property, the stubs will be saved in your user profile folder. Specify 'useInMemoryStorage' to only save stubs in memory.",
        Example = @"C:\httplaceholder_storage",
        ConfigPath = "Storage:FileStorageLocation",
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string FileStorageLocationKey = "fileStorageLocation";

    /// <summary>
    /// Constant for useInMemoryStorage.
    /// </summary>
    [ConfigKey(
        Description =
            "whether stubs should only be saved in memory. The stubs and requests will be deleted after restarting the application. Default: false",
        Example = "false",
        ConfigPath = "Storage:UseInMemoryStorage",
        IsBoolValue = true,
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string UseInMemoryStorage = "useInMemoryStorage";

    /// <summary>
    /// Constant for mysqlConnectionString.
    /// </summary>
    [ConfigKey(
        Description = "a connection string that needs to be filled in if you want to use MySQL",
        Example =
            "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder;Allow User Variables=true",
        ConfigPath = "ConnectionStrings:MySql",
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string MysqlConnectionStringKey = "mysqlConnectionString";

    /// <summary>
    /// Constant for sqliteConnectionString.
    /// </summary>
    [ConfigKey(
        Description = "a connection string that needs to be filled in if you want to use SQLite",
        Example = @"Data Source=C:\tmp\httplaceholder.db",
        ConfigPath = "ConnectionStrings:Sqlite",
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string SqliteConnectionStringKey = "sqliteConnectionString";

    /// <summary>
    /// Constant for apiUsername.
    /// </summary>
    [ConfigKey(
        Description = "a connection string that needs to be filled in if you want to use MSSQL",
        Example = "Server=localhost,2433;Database=httplaceholder;User Id=sa;Password=Password123",
        ConfigPath = "ConnectionStrings:SqlServer",
        ConfigKeyType = ConfigKeyType.Storage)]
    public const string SqlServerConnectionStringKey = "sqlServerConnectionString";

    /// <summary>
    /// Constant for enableUserInterface.
    /// </summary>
    [ConfigKey(
        Description =
            "whether the user interface should be enabled or not. The user interface is, if enabled, located at http://localhost:PORT/ph-ui.",
        Example = "true",
        ConfigPath = "Gui:EnableUserInterface",
        IsBoolValue = true,
        ConfigKeyType = ConfigKeyType.Gui)]
    public const string EnableUserInterface = "enableUserInterface";

    /// <summary>
    /// Constant for maximumExtraDurationMillisKey.
    /// </summary>
    [ConfigKey(
        Description =
            "the number of milliseconds the 'extraDuration' response writer can make a request wait at most. Defaults to 1 minute (60.000 millis).",
        Example = "60000",
        ConfigPath = "Stub:MaximumExtraDurationMillis",
        ConfigKeyType = ConfigKeyType.Stub)]
    public const string MaximumExtraDurationMillisKey = "maximumExtraDurationMillisKey";

    /// <summary>
    /// Constant for healthcheckOnRootUrl.
    /// </summary>
    [ConfigKey(
        Description =
            @"whether the root URL of HttPlaceholder (so ""/"") can be configured as stub or always returns 200 OK. Defaults to false.",
        Example = "true",
        ConfigPath = "Stub:HealthcheckOnRootUrl",
        IsBoolValue = true,
        ConfigKeyType = ConfigKeyType.Stub)]
    public const string HealthcheckOnRootUrl = "healthcheckOnRootUrl";

    /// <summary>
    /// Constant for configJsonLocation.
    /// </summary>
    [ConfigKey(
        Description =
            "the location of the config.json file. This JSON file contains all possible configuration settings and a default value per setting. You can copy this file to any location on your PC. Don't put the config file in the installation folder, because these files will be overwritten when an update is installed.",
        Example = @"F:\httplaceholder\config.json",
        ConfigKeyType = ConfigKeyType.Configuration)]
    public const string ConfigJsonLocationKey = "configJsonLocation";

    /// <summary>
    /// Returns a dictionary of <see cref="ConfigKeyType"/> and translation combinations.
    /// </summary>
    /// <returns>A dictionary of <see cref="ConfigKeyType"/> and translation combinations.</returns>
    public static IDictionary<ConfigKeyType, string> GetConfigKeyTypes() =>
        new Dictionary<ConfigKeyType, string>
        {
            {ConfigKeyType.Authentication, "Authentication"},
            {ConfigKeyType.Web, "Web"},
            {ConfigKeyType.Storage, "Storage"},
            {ConfigKeyType.Gui, "GUI"},
            {ConfigKeyType.Stub, "Stub"},
            {ConfigKeyType.Configuration, "Configuration"}
        };
}
