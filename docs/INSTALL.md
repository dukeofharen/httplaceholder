# Installation

## Windows

### Local development setup

The only thing needed to use HttPlaceholder on your local development machine, is executing the installer (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)). To get started, learn more about configuring the application or just curious about some samples, read [this](GETTING-STARTED.md), [this](CONFIG.md) and [this](SAMPLES.md).

### Hosting under IIS

- It is also possible to host HttPlaceholder under IIS. You can just install HttPlaceholder using the installer mentioned above.
- You need to install the .NET Core Hosting Bundle in order for you to host .NET Core applications under IIS. You can find the installer at <https://www.microsoft.com/net/download/dotnet-core/2.1>, under *Runtime & Hosting Bundle*.
- Additionally, two additional files are also installed on the machine: `_config.json` and `_web.config`. If you want to host the application in IIS, you have to remove the `_` from the filename. The `_` is added to the files so the actual configuration files won't be overwritten if a new version of HttPlaceholder is installed.
- The `web.config` is needed to instruct IIS how to host the application. The `config.json` file contains all possible configuration values that are also available through the command line. For more information on the `config.json` file, read more [here](CONFIG.md). Besides this, you might want the application logging to be written to a file. You can set the `stdoutLogFile` variable in the `web.config` file to a path on your disk and set `stdoutLogEnabled` to `true`.

### Hosting as a Windows Service

Hosting the application as a Windows Service (and subsequently using a reverse proxy in IIS to host the application) is officially not supported (maybe in the future). You can, however, use tools like [NSSM](http://nssm.cc/) (Non-Sucking Service Manager, brilliant name by the way) to host a console application as a Windows service. For configuration, you can either use the `config.json` file or the command line arguments.