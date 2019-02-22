# Installation

This page contains information about installing HttPlaceholder in several different ways and on several different operating systems. To get started, learn more about configuring the application or just curious about some samples, read [this](GETTING-STARTED.md), [this](CONFIG.md)  and [this](SAMPLES.md).

## ⚙ Dotnet global tool (cross platform)

Make sure you have installed the correct .NET Core SDK for your OS (see https://dotnet.microsoft.com/download). When the .NET Core SDK is installed, run `dotnet install --global httplaceholder` to install HttPlaceholder.

## 🐧 Linux

To install HttPlaceholder on Linux, run the following command in your terminal (make sure you're running as administrator):

```bash
curl -o- https://raw.githubusercontent.com/dukeofharen/httplaceholder/scripts/install-linux.sh | bash
```

If you would like to expose HttPlaceholder to the outside world, I would recommend to use Nginx or Apache as reverse proxy. To keep the service running even if you're not logged in through an SSH session, you can use something like systemd.

## 🍎 Mac

I haven't got access to Mac OS, so I haven't managed to test HttPlaceholder on Mac yet, but it (probably) should work. Download the latest Mac OS artifacts from the releases tab, extract it and it should run.

## 🗔 Windows

### Local development setup

The only thing needed to use HttPlaceholder on your local development machine, is executing the installer (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)).

For installing HttPlaceholder through your PowerShell terminal, execute the following command:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/dukeofharen/httplaceholder/scripts/Install-Windows.ps1'))
```

### Hosting under IIS

- It is also possible to host HttPlaceholder under IIS. You can just install HttPlaceholder using the installer mentioned above.
- You need to install the .NET Core Hosting Bundle in order for you to host .NET Core applications under IIS. You can find the installer at <https://www.microsoft.com/net/download/dotnet-core/2.1>, under *Runtime & Hosting Bundle*.
- Additionally, two additional files are also installed on the machine: `_config.json` and `_web.config`. If you want to host the application in IIS, you have to remove the `_` from the filename. The `_` is added to the files so the actual configuration files won't be overwritten if a new version of HttPlaceholder is installed.
- The `web.config` is needed to instruct IIS how to host the application. The `config.json` file contains all possible configuration values that are also available through the command line. For more information on the `config.json` file, read more [here](CONFIG.md). Besides this, you might want the application logging to be written to a file. You can set the `stdoutLogFile` variable in the `web.config` file to a path on your disk and set `stdoutLogEnabled` to `true`.

### Hosting as a Windows Service

Hosting the application as a Windows Service (and subsequently using a reverse proxy in IIS to host the application) is officially not supported (maybe in the future). You can, however, use tools like [NSSM](http://nssm.cc/) (Non-Sucking Service Manager, brilliant name by the way) to host a console application as a Windows service. For configuration, you can either use the `config.json` file or the command line arguments.