using System.Collections.Generic;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Domain.Enums;
using static HttPlaceholder.Application.Configuration.Models.ConfigMetadataModel;

namespace HttPlaceholder.Application.Configuration;

/// <summary>
///     A class that contains constants for every possible configuration option.
/// </summary>
public static class ConfigKeys
{
    /// <summary>
    ///     Constant for dev.
    /// </summary>
    public const string Dev = "dev";

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
    ///     Constant for disableFileWatcher.
    /// </summary>
    public const string DisableFileWatcher = "disableFileWatcher";

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
    ///     Constant for publicUrl.
    /// </summary>
    public const string PublicUrl = "publicUrl";

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
    ///     Constant for sqlServerConnectionString.
    /// </summary>
    public const string SqlServerConnectionStringKey = "sqlServerConnectionString";

    /// <summary>
    ///     Constant for sqlServerConnectionString.
    /// </summary>
    public const string PostgresConnectionStringKey = "postgresConnectionString";

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
    ///     Constant for allowGlobalFileSearch.
    /// </summary>
    public const string AllowGlobalFileSearch = "allowGlobalFileSearch";

    /// <summary>
    ///     Constant for enableReverseProxy.
    /// </summary>
    public const string EnableReverseProxy = "enableReverseProxy";

    /// <summary>
    ///     Constant for allowedHosts.
    /// </summary>
    public const string AllowedHosts = "allowedHosts";

    /// <summary>
    ///     Constant for disallowedHosts.
    /// </summary>
    public const string DisallowedHosts = "disallowedHosts";

    /// <summary>
    ///     Constant for configJsonLocation.
    /// </summary>
    public const string ConfigJsonLocationKey = "configJsonLocation";

    private static readonly IDictionary<ConfigKeyType, string> _configKeyTypes = new Dictionary<ConfigKeyType, string>
    {
        { ConfigKeyType.Authentication, "Authentication" },
        { ConfigKeyType.Web, "Web" },
        { ConfigKeyType.Storage, "Storage" },
        { ConfigKeyType.Gui, "GUI" },
        { ConfigKeyType.Stub, "Stub" },
        { ConfigKeyType.Configuration, "Configuration" },
        { ConfigKeyType.Development, "Development" }
    };

    private static readonly ConfigMetadataModel[] _configMetadata =
    [
        Create(
            Dev,
            ConfigKeysResources.Dev,
            "user",
            "Development:DevModeEnabled",
            ConfigKeyType.Development,
            true,
            null,
            null),
        Create(
            ApiUsernameKey,
            ConfigKeysResources.ApiUsername,
            "user",
            "Authentication:ApiUsername",
            ConfigKeyType.Authentication,
            null,
            null,
            null),
        Create(
            ApiPasswordKey,
            ConfigKeysResources.ApiPassword,
            "pass",
            "Authentication:ApiPassword",
            ConfigKeyType.Authentication,
            null,
            true,
            null),
        Create(
            HttpsPortKey,
            ConfigKeysResources.HttpsPort,
            "5050",
            "Web:HttpsPort",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            InputFileKey,
            ConfigKeysResources.InputFile,
            @"C:\path\to\stubsfolder or C:\path\to\stubsfolder,C:\path\to\file.yml for multiple paths",
            "Storage:InputFile",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            DisableFileWatcher,
            ConfigKeysResources.DisableFileWatcher,
            "true",
            "Storage:DisableFileWatcher",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            OldRequestsQueueLengthKey,
            ConfigKeysResources.OldRequestsQueueLength,
            "100",
            "Storage:OldRequestsQueueLength",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            StoreResponses,
            ConfigKeysResources.StoreResponses,
            "true",
            "Storage:StoreResponses",
            ConfigKeyType.Storage,
            true,
            null,
            true),
        Create(
            CleanOldRequestsInBackgroundJob,
            ConfigKeysResources.CleanOldRequestsInBackgroundJob,
            "true",
            "Storage:CleanOldRequestsInBackgroundJob",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            PfxPasswordKey,
            ConfigKeysResources.PfxPassword,
            "112233",
            "Web:PfxPassword",
            ConfigKeyType.Web,
            null,
            true,
            null),
        Create(
            ReadProxyHeaders,
            ConfigKeysResources.ReadProxyHeaders,
            "true",
            "Web:ReadProxyHeaders",
            ConfigKeyType.Web,
            true,
            null,
            null),
        Create(
            SafeProxyIps,
            ConfigKeysResources.SafeProxyIps,
            "1.1.1.1,2.2.2.2",
            "Web:SafeProxyIps",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            PfxPathKey,
            ConfigKeysResources.PfxPath,
            @"C:\path\to\privatekey.pfx",
            "Web:PfxPath",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            PortKey,
            ConfigKeysResources.Port,
            "5000",
            "Web:HttpPort",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            UseHttpsKey,
            ConfigKeysResources.UseHttps,
            "true",
            "Web:UseHttps",
            ConfigKeyType.Web,
            true,
            null,
            null),
        Create(
            PublicUrl,
            ConfigKeysResources.PublicUrl,
            "https://example.com/stubs",
            "Web:PublicUrl",
            ConfigKeyType.Web,
            null,
            null,
            null),
        Create(
            EnableRequestLoggingKey,
            ConfigKeysResources.EnableRequestLogging,
            "false",
            "Storage:EnableRequestLogging",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            FileStorageLocationKey,
            ConfigKeysResources.FileStorageLocation,
            @"C:\httplaceholder_storage",
            "Storage:FileStorageLocation",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            UseInMemoryStorage,
            ConfigKeysResources.UseInMemoryStorage,
            "false",
            "Storage:UseInMemoryStorage",
            ConfigKeyType.Storage,
            true,
            null,
            null),
        Create(
            MysqlConnectionStringKey,
            ConfigKeysResources.MysqlConnectionString,
            "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder;Allow User Variables=true",
            "ConnectionStrings:MySql",
            ConfigKeyType.Storage,
            null,
            true,
            null),
        Create(
            SqliteConnectionStringKey,
            ConfigKeysResources.SqliteConnectionString,
            @"Data Source=C:\tmp\httplaceholder.db",
            "ConnectionStrings:Sqlite",
            ConfigKeyType.Storage,
            null,
            null,
            null),
        Create(
            SqlServerConnectionStringKey,
            ConfigKeysResources.SqlServerConnectionString,
            "Server=localhost,2433;Database=httplaceholder;User Id=sa;Password=Password123",
            "ConnectionStrings:SqlServer",
            ConfigKeyType.Storage,
            null,
            true,
            null),
        Create(
            PostgresConnectionStringKey,
            ConfigKeysResources.PostrgresConnectionString,
            "Host=localhost,5432;Username=postgres;Password=postgres;Database=httplaceholder;SearchPath=public",
            "ConnectionStrings:Postgres",
            ConfigKeyType.Storage,
            null,
            true,
            null),
        Create(
            EnableUserInterface,
            ConfigKeysResources.EnableUserInterface,
            "true",
            "Gui:EnableUserInterface",
            ConfigKeyType.Gui,
            true,
            null,
            null),
        Create(
            MaximumExtraDurationMillisKey,
            ConfigKeysResources.MaximumExtraDurationMillis,
            "60000",
            "Stub:MaximumExtraDurationMillis",
            ConfigKeyType.Stub,
            null,
            null,
            null),
        Create(
            HealthcheckOnRootUrl,
            ConfigKeysResources.HealthcheckOnRootUrl,
            "true",
            "Stub:HealthcheckOnRootUrl",
            ConfigKeyType.Stub,
            true,
            null,
            null),
        Create(
            AllowGlobalFileSearch,
            ConfigKeysResources.AllowGlobalFileSearch,
            "false",
            "Stub:AllowGlobalFileSearch",
            ConfigKeyType.Stub,
            true,
            null,
            null),
        Create(
            EnableReverseProxy,
            ConfigKeysResources.EnableReverseProxy,
            "true",
            "Stub:EnableReverseProxy",
            ConfigKeyType.Stub,
            true,
            null,
            null),
        Create(
            AllowedHosts,
                ConfigKeysResources.AllowedHosts,
            "^google\\.com$,www.google.com",
            "Stub:AllowedHosts",
            ConfigKeyType.Stub,
            true,
            null,
            null),
        Create(
            DisallowedHosts,
            ConfigKeysResources.DisallowedHosts,
            "^reddit\\.com$,www.reddit.com",
            "Stub:DisallowedHosts",
            ConfigKeyType.Stub,
            true,
            null,
            null),
        Create(
            ConfigJsonLocationKey,
            ConfigKeysResources.ConfigJsonLocation,
            @"F:\httplaceholder\config.json",
            null,
            ConfigKeyType.Configuration,
            null,
            null,
            null)
    ];

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
