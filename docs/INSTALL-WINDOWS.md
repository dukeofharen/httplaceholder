## Install HttPlaceholder on Windows

### Local development setup

The only thing needed to use HttPlaceholder on your local development machine, is extracting the archive with the HttPlaceholder binaries (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)).

For installing HttPlaceholder through your PowerShell terminal, execute the following command:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/dukeofharen/httplaceholder/master/scripts/Install-Windows.ps1'))
```

This is a self contained version of HttPlaceolder: no SDK has to be installed to run it.

### Hosting under IIS

It is also possible to host HttPlaceholder under IIS. You can just install HttPlaceholder using the installer mentioned above. You need to install the .NET Hosting Bundle in order for you to host .NET applications under IIS. You can find the installer at <https://dotnet.microsoft.com/download/dotnet-core/current/runtime>, and download the Hosting Bundle.

You need, of course, to download HttPlaceholder to your Windows machine. Just follow the instructions above. Besides that, make sure IIS is installed.

If you've completed the steps above, execute the following steps:

- Create a new site in IIS and (optionally) fill in a hostname and the location to the HttPlaceholder binaries.
![Step 1](img/iis_step1.png)

- Now, you need to setup the configuration. For this, you need to rename the file `_web.config` in the HttPlaceholder installation folder to `web.config`. You can modify the `web.config` to look like this.

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore
           processPath=".\HttPlaceholder.exe"
           arguments="-V --sqliteConnectionString Data Source=C:\\httplaceholderdata\\httplaceholder.db "
           stdoutLogEnabled="true"
           stdoutLogFile="C:\logs\httplaceholder"
           hostingModel="inprocess" />
    </system.webServer>
  </location>
</configuration>
```

In the example above, all the standard output logging will be written to a file and HttPlaceholder is configured to store its data in a SQLite database (all the configuration values are explained [here](CONFIG.md)). While this seems like a nice solution, if you have multiple configuration items, it might be better if you create a separate `config.json` file and point to that file in your `web.config`. You then might have these two files:

*config.json*
```
{
    "sqliteConnectionString": "Data Source=C:\httplaceholderdata\httplaceholder.db"
}
``` 

*web.config*
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore
           processPath=".\HttPlaceholder.exe"
           arguments="-V --configjsonlocation C:\\httplaceholderdata\\config.json"
           stdoutLogEnabled="true"
           stdoutLogFile="C:\logs\httplaceholder"
           hostingModel="inprocess" />
    </system.webServer>
  </location>
</configuration>
```

In this case, all the configuration is kept in a separate JSON file which is referred to in the `web.config`.

If you go to the hostname + port as specified in the IIS site and everything went well, you will go to HttPlaceholder (open the `/ph-ui` path to verify it works).

There are two Vagrant boxes (Windows and Ubuntu) that you can use to view how installation of HttPlaceholder is done. You can find them in this repository under the folder "vagrant". You need to have [Vagrant](https://www.vagrantup.com/) installed. After that, it's just a matter of going to the correct folder in your terminal and typing `vagrant up`. HttPlaceholder will then be installed under Windows or Ubuntu and can be reached by going to `http://localhost:8080` or `https://localhost:4430`.

#### Troubleshooting

- If you get an error something like `An unhandled exception was thrown by the application. code = ReadOnly (8), message = System.Data.SQLite.SQLiteException (0x800017FF): attempt to write a readonly database`, it means your SQLite database is not writable. Make sure the IIS user can write to this file.

### Hosting as a Windows Service

Hosting the application as a Windows Service (and subsequently using a reverse proxy in IIS to host the application) is officially not supported (maybe in the future). You can, however, use tools like [NSSM](http://nssm.cc/) (Non-Sucking Service Manager, brilliant name by the way) to host a console application as a Windows service. For configuration, you can either use the `config.json` file or the command line arguments.