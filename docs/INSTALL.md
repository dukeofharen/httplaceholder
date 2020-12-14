# Installation

This page contains information about installing HttPlaceholder in several different ways and on several different operating systems. To get started, learn more about configuring the application or just curious about some samples, read [this](GETTING-STARTED.md), [this](CONFIG.md)  and [this](SAMPLES.md).

## Dotnet global tool (cross platform)

Make sure you have installed the correct .NET SDK (at least .NET 5) for your OS (see https://dotnet.microsoft.com/download). When the .NET SDK is installed, run `dotnet tool install --global httplaceholder` to install HttPlaceholder.

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

## Using SSL

HttPlaceholder supports HTTPS. See [configuration](CONFIG.md) for more information on this. By default, it uses the private key that is installed with HttPlaceholder. This file is named `key.pfx` and the password is `1234`. Before using HttPlaceholder and calling the HTTPS URL, you'll need to make sure to import and trust the .pfx file on your OS. For your convenience, three scripts (for Windows, Linux and Mac) are added for installing and trusting the .pfx file of HttPlaceholder. You can find the script in the installation folder: `install-private-key.sh` for Mac and Linux and `Install-Private-Key.ps1` for Windows.