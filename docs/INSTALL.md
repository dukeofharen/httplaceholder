# Installation

This page contains information about installing HttPlaceholder in several different ways and on several different operating systems. To get started, learn more about configuring the application or just curious about some samples, read [this](GETTING-STARTED.md), [this](CONFIG.md)  and [this](SAMPLES.md).

## Dotnet global tool (cross platform)

Make sure you have installed the correct .NET SDK (at least .NET 5) for your OS (see https://dotnet.microsoft.com/download). When the .NET SDK is installed, run `dotnet tool install --global httplaceholder` to install HttPlaceholder.

## Windows

### Local development setup

The only thing needed to use HttPlaceholder on your local development machine, is extracting the archive with the HttPlaceholder binaries (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)).

For installing HttPlaceholder through your PowerShell terminal, execute the following command:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/dukeofharen/httplaceholder/master/scripts/Install-Windows.ps1'))
```

This is a self contained version of HttPlaceolder: no SDK has to be installed to run it.

### Hosting under IIS

- It is also possible to host HttPlaceholder under IIS. You can just install HttPlaceholder using the installer mentioned above.
- You need to install the .NET Hosting Bundle in order for you to host .NET applications under IIS. You can find the installer at <https://dotnet.microsoft.com/download/dotnet-core/current/runtime>, and download the Hosting Bundle.
- Additionally, two additional files are also installed on the machine: `_config.json` and `_web.config`. If you want to host the application in IIS, you have to remove the `_` from the filename. The `_` is added to the files so the actual configuration files won't be overwritten if a new version of HttPlaceholder is installed.
- The `web.config` is needed to instruct IIS how to host the application. The `config.json` file contains all possible configuration values that are also available through the command line. This file needs to be moved to a location outside of the installation directory. For more information on the `config.json` file, read more [here](CONFIG.md). Besides this, you might want the application logging to be written to a file. You can set the `stdoutLogFile` variable in the `web.config` file to a path on your disk and set `stdoutLogEnabled` to `true`.

### Hosting as a Windows Service

Hosting the application as a Windows Service (and subsequently using a reverse proxy in IIS to host the application) is officially not supported (maybe in the future). You can, however, use tools like [NSSM](http://nssm.cc/) (Non-Sucking Service Manager, brilliant name by the way) to host a console application as a Windows service. For configuration, you can either use the `config.json` file or the command line arguments.

## Docker

HttPlaceholder has an official Docker image. Read more about it [here](INSTALL-DOCKER.md).

## Running behind reverse proxy

IIS, Nginx and Apache (and a lot of other web servers) have the option to run an application behind a reverse proxy. For HttPlaceholder to function correctly behind a reverse proxy, the server has to send a few "proxy" headers to HttPlaceholders. The following headers should be sent:

- `X-Forwarded-For`: contains all IP addresses of the calling client and all proxy servers in between the client and HttPlaceholder. Used to determine the IP of the calling client.
- `X-Forwarded-Proto`: contains the protocal of the original call to the proxying web server (`http` or `https`).
- `X-Forwarded-Host`: contains the hostname of the original call to the proxying web server(e.g. `httplaceholder.com`).

These headers are, right now, only used instead of the "real" values if the actual IP address of the proxy server is the loopback IP (e.g. `127.0.0.1`, `::1` etc.).

Read more about this subject for the specific web servers:
- [Nginx](https://www.nginx.com/resources/wiki/start/topics/examples/forwarded/)
- [Apache](https://httpd.apache.org/docs/2.4/mod/mod_proxy.html)
- [IIS](https://blogs.msdn.microsoft.com/webapps/2018/09/05/how-to-log-client-ip-when-iis-is-load-balanced-the-x-forwarded-for-header-xff/)

## Vagrant (virtual machines)

There are two Vagrant boxes (Windows and Ubuntu) that you can use to view how installation of HttPlaceholder is done. You can find them in this repository under the folder "vagrant". You need to have [Vagrant](https://www.vagrantup.com/) installed. After that, it's just a matter of going to the correct folder in your terminal and typing `vagrant up`. HttPlaceholder will then be installed under Windows or Ubuntu and can be reached by going to `http://localhost:8080` or `https://localhost:4430`.

## Using SSL

HttPlaceholder supports HTTPS. See [configuration](CONFIG.md) for more information on this. By default, it uses the private key that is installed with HttPlaceholder. This file is named `key.pfx` and the password is `1234`. Before using HttPlaceholder and calling the HTTPS URL, you'll need to make sure to import and trust the .pfx file on your OS. For your convenience, three scripts (for Windows, Linux and Mac) are added for installing and trusting the .pfx file of HttPlaceholder. You can find the script in the installation folder: `install-private-key.sh` for Mac and Linux and `Install-Private-Key.ps1` for Windows.