using System.Text.RegularExpressions;
using HttPlaceholder.Models.Attributes;

namespace HttPlaceholder.Models
{
    public static class Constants
    {
        public static class ConfigKeys
        {
            [ConfigKey(Description = "the username for securing the REST API", Example = "user")]
            public const string ApiUsernameKey = "apiUsername";

            [ConfigKey(Description = "the password for securing the REST API", Example = "pass")]
            public const string ApiPasswordKey = "apiPassword";

            [ConfigKey(Description = "the port HttPlaceholder should run under when HTTPS is enabled", Example = "5050")]
            public const string HttpsPortKey = "httpsPort";

            [ConfigKey(Description = "for input file, you can both provide a path to a .yml file (to load only that file) or provide a path to a folder containing .yml files (which will all be loaded in that case)", Example = @"C:\path\to\stubsfolder or C:\path\to\stubsfolder%%C:\path\to\file.yml for multiple paths")]
            public const string InputFileKey = "inputFile";

            [ConfigKey(Description = "the maximum number of HTTP requests that the in memory stub source (used for the REST API) should store before truncating old records. Default: 40.", Example = "100")]
            public const string OldRequestsQueueLengthKey = "oldRequestsQueueLength";

            [ConfigKey(Description = "the password for the .pfx file which should be used in the case HTTPS is enabled", Example = "112233")]
            public const string PfxPasswordKey = "pfxPassword";

            [ConfigKey(Description = "the path to the .pfx file in the case HTTPS is enabled. When no path is provided, the default .pfx file is used", Example = @"C:\path\to\privatekey.pfx")]
            public const string PfxPathKey = "pfxPath";

            [ConfigKey(Description = "the HTTP port HttPlaceholder should run under", Example = "5000")]
            public const string PortKey = "port";

            [ConfigKey(Description = "whether HTTPS should be enabled or not. Default: false", Example = "true")]
            public const string UseHttpsKey = "useHttps";

            [ConfigKey(Description = "whether stub requests should be logged to the terminal window. Default: false", Example = "false")]
            public const string EnableRequestLoggingKey = "enableRequestLogging";

            [ConfigKey(Description = "a location where the stubs and requests are stored", Example = @"C:\httplaceholder_storage")]
            public const string FileStorageLocationKey = "fileStorageLocation";

            [ConfigKey(Description = "a connection string that needs to be filled in if you want to use MySQL", Example = "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder;Allow User Variables=true")]
            public const string MysqlConnectionStringKey = "mysqlConnectionString";

            [ConfigKey(Description = "a connection string that needs to be filled in if you want to use SQLite", Example = @"Data Source=C:\tmp\httplaceholder.db")]
            public const string SqliteConnectionStringKey = "sqliteConnectionString";

            [ConfigKey(Description = "a connection string that needs to be filled in if you want to use MSSQL", Example = "Server=localhost,2433;Database=httplaceholder;User Id=sa;Password=Password123")]
            public const string SqlServerConnectionStringKey = "sqlServerConnectionString";

            [ConfigKey(Description = "the location of the config.json file. This JSON file contains all possible configuration settings and a default value per setting. You can copy this file to any location on your PC. Don't put the config file in the installation folder, because these files will be overwritten when an update is installed.", Example = @"F:\httplaceholder\config.json")]
            public const string ConfigJsonLocationKey = "configJsonLocation";
        }

        public static class CookieKeys
        {
            public const string LoginCookieKey = "HttPlaceholderLoggedin";
        }
    }
}
