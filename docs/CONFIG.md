# Configuration

This page contains all command line arguments supported by HttPlaceholder. Configuration can be set using command line arguments, a configuration file or environment variables.

## Command line arguments

### Verbose output

If you want some more logging, append `-V` or `--verbose` as argument.

```bash
httplaceholder --verbose
```

### Get version

If you want to check the HttPlaceholder version, append `-v` or `--version` as argument.

```bash
httplaceholder --version
```

### Get help

If you want to see all possible configuration parameters, append `-h`, `-?` or `--help` as argument.

```bash
httplaceholder --help
```

### Input file (optional)

```bash
httplaceholder --inputFile C:\path\to\file.yml
```

```bash
httplaceholder --inputFile C:\path\to\stubsfolder
```

For input file, you can both provide a path to a .yml file (to load only that file) or provide a path to a folder containing .yml files (which will all be loaded in that case).

If you want to provide multiple paths (be it folders or files), that's also possible. You can do it like this:

```bash
httplaceholder --inputFile "C:\path\to\stubsfolder,C:\path\to\file.yml"
```

Make sure to surround the value with double quotes if you run the command from PowerShell.

### File store (optional)

```bash
httplaceholder --fileStorageLocation C:\tmp\storage
```

This configuration value can be used to configure where you want to save the stubs (which are created through the API). The files will be saved as plain JSON files. 

By default, if you start the application, a folder called `.httplaceholder` will be added to your user profile folder. On Windows, this will be `C:\Users\<username>\.httplaceholder` and on Unix like systems (in this case Linux and OS X) this will be `/home/<username>/.httplaceholder`. If HttPlaceholder couldn't determine your OS or your profile folder is in a non-standard location, the "in memory stub source" is used, which means your data will be cleared when you start HttPlaceholder.

### Use in memory storage (optional)

```bash
httplaceholder --useInMemoryStorage
```

If you specifically don't want the stubs and requests to be stored on a static location (e.g. disk or database), you can specify this option so all your data will be cleared whenever the application is restarted. This can be handy in test scenarios where HttPlaceholder will not be running for a long time.

### MySQL connection (optional)

<img src="img/mysql.png" width="100" />

```bash
httplaceholder --mysqlConnectionString "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder;Allow User Variables=true"
```

HttPlaceholder has functionality to save all requests and stubs to a MySQL database. You can connect to a database by providing a connection string as seen above. You already need to have an empty database created for HttPlaceholder. HttPlaceholder will create tables itself (if they aren't created yet). `Allow User Variables` should be set to `true` because the initialization script uses variables.

### SQLite connection (optional)

<img src="img/sqlite.png" width="100" />

```bash
httplaceholder --sqliteConnectionString "Data Source=C:\tmp\httplaceholder.db"
```

HttPlaceholder has functionality to save all requests and stubs to a SQLite database. You can use SQLite by providing the connection string as seen above. HttPlaceholder will create the file if it doesn't exist and will populate the database with the necessary tables.

### SQL Server connection (optional)

<img src="img/mssql.png" width="100" />

```bash
httplaceholder --sqlServerConnectionString "Server=localhost,2433;Database=httplaceholder;User Id=sa;Password=Password123"
```

HttPlaceholder has functionality to save all requests and stubs to a Microsoft SQL Server database. You can connect to a database by providing a connection string as seen above. You already need to have an empty database created for HttPlaceholder. HttPlaceholder will create tables itself (if they aren't created yet).

### Use HTTPS (optional)

```bash
httplaceholder --useHttps true
```

Whether to also use HTTPS. Possible values: `true` or `false`. Default: `false`

### HTTPS certificates (optional)

```bash
httplaceholder --pfxPath C:\path\to\privatekey.pfx --pfxPassword 11223344
```

Define the private key used for hosting the stub with HTTPS. If no pfx path and pfx password are set, the default .pfx file, shipped with HttPlaceholder, is used.

### HTTP(S) port (optional)

```bash
httplaceholder --port 80 --httpsPort 443 --useHttps true
```

Defines which ports the stub should be available at. Default value of `port` (the HTTP port) is `5000` and default value of `httpsPort` is `5050`.

You can define multiple ports for HttPlaceholder to listen on. To do this, separate the port numbers with ",", like this:

```bash
httplaceholder --port "80,81" --httpsPort "443,4430" --useHttps true
```

### Request logging (optional)

```bash
httplaceholder --oldRequestsQueueLength 100
```

The maximum number of HTTP requests that the in memory stub source (used for the REST API) should store before truncating old records. Default: 40.

### REST API Authentication (optional)

```bash
httplaceholder --apiUsername user --apiPassword pass
```

The username and password that should be sent (using basic authentication) when communicating with the REST API. If these values are not set, the API is available for everyone.

### Enable / disable request logging on the terminal (optional)

```bash
httplaceholder --enableRequestLogging false
```

If this property is set to false, no detailed request logging will be written to the terminal anymore. Default: true.

### Enable or disable user interface

```bash
httplaceholder --enableUserInterface false
```

If this property is set to false, the user interface won't appear when you go to http://httplaceholderhost:port/ph-ui. This might be handy in situations where you only want to deploy the HttPlaceholder application as API / stub engine. 

### Config JSON location

If you just installed HttPlaceholder, a file called `_config.json` is available in the installation folder. This JSON file contains all possible configuration settings and a default value per setting. You can copy this file to any location on your PC. Don't put the config file in the installation folder, because these files will be overwritten when an update is installed.

```json
{
    "apiUsername": null,
    "apiPassword": null,
    "httpsPort": "5050",
    "inputFile": null,
    "oldRequestsQueueLength": 40,
    "pfxPassword": null,
    "pfxPath": null,
    "port": "5000",
    "useHttps": false,
    "enableRequestLogging": true,
    "fileStorageLocation": "C:\\tmp\\storage",
    "mysqlConnectionString": "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder;Allow User Variables=true",
    "sqliteConnectionString": "Data Source=C:\\tmp\\httplaceholder.db",
    "sqlServerConnectionString": "Server=localhost,2433;Database=httplaceholder;User Id=sa;Password=Password123",
    "useInMemoryStorage": false,
    "enableUserInterface": true
}
```

You can tell HttPlaceholder to use this file, instead of all separate command line arguments, like this:

```bash
httplaceholder --configJsonLocation F:\httplaceholder\config.json
```