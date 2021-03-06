﻿using System.Diagnostics.CodeAnalysis;
using HttPlaceholder.Application.Configuration.Attributes;

namespace HttPlaceholder.Application.Configuration
{
    public static class ConfigKeys
    {
        [ConfigKey(
            Description = "the username for securing the REST API",
            Example = "user",
            ConfigPath = "Authentication:ApiUsername")]
        public const string ApiUsernameKey = "apiUsername";

        [SuppressMessage("SonarQube", "S2068", Justification = "Not a password, just a configuration key.")]
        [ConfigKey(
            Description = "the password for securing the REST API",
            Example = "pass",
            ConfigPath = "Authentication:ApiPassword")]
        public const string ApiPasswordKey = "apiPassword";

        [ConfigKey(
            Description = "the port HttPlaceholder should run under when HTTPS is enabled",
            Example = "5050",
            ConfigPath = "Web:HttpsPort")]
        public const string HttpsPortKey = "httpsPort";

        [ConfigKey(
            Description = "for input file, you can both provide a path to a .yml file (to load only that file) or provide a path to a folder containing .yml files (which will all be loaded in that case).",
            Example = @"C:\path\to\stubsfolder or C:\path\to\stubsfolder,C:\path\to\file.yml for multiple paths",
            ConfigPath = "Storage:InputFile")]
        public const string InputFileKey = "inputFile";

        [ConfigKey(
            Description = "the maximum number of HTTP requests that the in memory stub source (used for the REST API) should store before truncating old records. Default: 40.",
            Example = "100",
            ConfigPath = "Storage:OldRequestsQueueLength")]
        public const string OldRequestsQueueLengthKey = "oldRequestsQueueLength";

        [SuppressMessage("SonarQube", "S2068", Justification = "Not a password, just a configuration key.")]
        [ConfigKey(
            Description = "the password for the .pfx file which should be used in the case HTTPS is enabled",
            Example = "112233",
            ConfigPath = "Web:PfxPassword")]
        public const string PfxPasswordKey = "pfxPassword";

        [ConfigKey(
            Description = "the path to the .pfx file in the case HTTPS is enabled. When no path is provided, the default .pfx file is used",
            Example = @"C:\path\to\privatekey.pfx",
            ConfigPath = "Web:PfxPath")]
        public const string PfxPathKey = "pfxPath";

        [ConfigKey(
            Description = "the HTTP port HttPlaceholder should run under",
            Example = "5000",
            ConfigPath = "Web:HttpPort")]
        public const string PortKey = "port";

        [ConfigKey(
            Description = "whether HTTPS should be enabled or not. Default: false",
            Example = "true",
            ConfigPath = "Web:UseHttps",
            IsBoolValue = true)]
        public const string UseHttpsKey = "useHttps";

        [ConfigKey(
            Description = "whether stub requests should be logged to the terminal window. Default: false",
            Example = "false",
            ConfigPath = "Storage:EnableRequestLogging",
            IsBoolValue = true)]
        public const string EnableRequestLoggingKey = "enableRequestLogging";

        [ConfigKey(
            Description = "a location where the stubs and requests are stored. This is enabled by default and, if you omit this property, the stubs will be saved in your user profile folder. Specify 'useInMemoryStorage' to only save stubs in memory.",
            Example = @"C:\httplaceholder_storage",
            ConfigPath = "Storage:FileStorageLocation")]
        public const string FileStorageLocationKey = "fileStorageLocation";

        [ConfigKey(
            Description = "whether stubs should only be saved in memory. The stubs and requests will be deleted after restarting the application. Default: false",
            Example = "false",
            ConfigPath = "Storage:UseInMemoryStorage",
            IsBoolValue = true)]
        public const string UseInMemoryStorage = "useInMemoryStorage";

        [ConfigKey(
            Description = "a connection string that needs to be filled in if you want to use MySQL",
            Example = "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder;Allow User Variables=true",
            ConfigPath = "ConnectionStrings:MySql")]
        public const string MysqlConnectionStringKey = "mysqlConnectionString";

        [ConfigKey(
            Description = "a connection string that needs to be filled in if you want to use SQLite",
            Example = @"Data Source=C:\tmp\httplaceholder.db",
            ConfigPath = "ConnectionStrings:Sqlite")]
        public const string SqliteConnectionStringKey = "sqliteConnectionString";

        [ConfigKey(
            Description = "a connection string that needs to be filled in if you want to use MSSQL",
            Example = "Server=localhost,2433;Database=httplaceholder;User Id=sa;Password=Password123",
            ConfigPath = "ConnectionStrings:SqlServer")]
        public const string SqlServerConnectionStringKey = "sqlServerConnectionString";

        [ConfigKey(
            Description = "whether the user interface should be enabled or not. The user interface is, if enabled, located at http://localhost:PORT/ph-ui.",
            Example = "true",
            ConfigPath = "Gui:EnableUserInterface",
            IsBoolValue = true)]
        public const string EnableUserInterface = "enableUserInterface";

        [ConfigKey(
            Description = "the number of milliseconds the 'extraDuration' response writer can make a request wait at most. Defaults to 1 minute (60.000 millis).",
            Example = "60000",
            ConfigPath = "Stub:MaximumExtraDurationMillis")]
        public const string MaximumExtraDurationMillisKey = "maximumExtraDurationMillisKey";

        [ConfigKey(
            Description = @"whether the root URL of HttPlaceholder (so ""/"") can be configured as stub or always returns 200 OK. Defaults to false.",
            Example = "true",
            ConfigPath = "Stub:HealthcheckOnRootUrl",
            IsBoolValue = true)]
        public const string HealthcheckOnRootUrl = "healthcheckOnRootUrl";

        [ConfigKey(
            Description = "the location of the config.json file. This JSON file contains all possible configuration settings and a default value per setting. You can copy this file to any location on your PC. Don't put the config file in the installation folder, because these files will be overwritten when an update is installed.",
            Example = @"F:\httplaceholder\config.json")]
        public const string ConfigJsonLocationKey = "configJsonLocation";
    }
}
