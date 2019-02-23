﻿using System.Text.RegularExpressions;

namespace HttPlaceholder.Models
{
    public static class Constants
    {
        public static class ConfigKeys
        {
            public const string ApiUsernameKey = "apiUsername";

            public const string ApiPasswordKey = "apiPassword";

            public const string HttpsPortKey = "httpsPort";

            public const string InputFileKey = "inputFile";

            public const string OldRequestsQueueLengthKey = "oldRequestsQueueLength";

            public const string PfxPasswordKey = "pfxPassword";

            public const string PfxPathKey = "pfxPath";

            public const string PortKey = "port";

            public const string UseHttpsKey = "useHttps";

            public const string EnableRequestLoggingKey = "enableRequestLogging";

            public const string FileStorageLocationKey = "fileStorageLocation";

            public const string MysqlConnectionStringKey = "mysqlConnectionString";

            public const string SqliteConnectionStringKey = "sqliteConnectionString";

            public const string SqlServerConnectionStringKey = "sqlServerConnectionString";

            public const string ConfigJsonLocationKey = "configJsonLocation";
        }

        public static class DefaultValues
        {
            public const int MaxRequestsQueueLength = 40;
        }

        public static class Regexes
        {
            public static Regex VarRegex = new Regex(@"\(\(([a-zA-Z0-9_]*)\:? ?([^)]*)?\)\)", RegexOptions.Compiled);
        }
    }
}