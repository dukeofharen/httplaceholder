TODO Replace .md references
TODO spell checking

# HttPlaceholder documentation

- [Installation](#installation)
  - [Dotnet global tool](#dotnet-global-tool-cross-platform)
  - [Windows](#windows)
  - [Linux](#linux)
  - [Mac](#mac)
  - [Docker](#docker)
  - [Hosting](#hosting)
- [Getting started](#getting-started)
- [Request conditions](#request-conditions)
  - [General](#general)
  - [Description](#description)
  - [Enabled](#enabled)
  - [Scenario](#request-scenario)
    - [Hit counter checking](#hit-counter-checking)
    - [State checking](#state-checking)
  - [Priority](#priority)
  - [URI](#uri)
    - [Path](#path)
    - [Full path](#full-path)
    - [Query string](#query-string)
    - [Is HTTPS](#is-https)
  - [HTTP method](#request-headers)
  - [Security](#security)
    - [Basic authentication](#basic-authentication)
  - [HTTP headers](#request-headers)
  - [Request body](#request-body)
    - [Raw body](#raw-body)
    - [Form](#form)
    - [JSON](#json)
    - [JSONPath](#jsonpath)
    - [XPath](#xpath)
  - [Client IP validation](#client-ip-validation)
  - [Hostname](#hostname)

# Installation

## Dotnet global tool (cross platform)

Make sure you have installed the correct .NET SDK (at least .NET 5) for your OS (see https://dotnet.microsoft.com/download). When the .NET SDK is installed, run `dotnet tool install --global httplaceholder` to install HttPlaceholder.

## Windows

### Local development setup

The only thing needed to use HttPlaceholder on your local development machine, is extracting the archive with the HttPlaceholder binaries (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)).

For installing HttPlaceholder through your PowerShell terminal, execute the following command:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/dukeofharen/httplaceholder/master/scripts/Install-Windows.ps1'))
```

This is a self contained version of HttPlaceholder: no SDK has to be installed to run it.

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

## Linux

The only thing needed to use HttPlaceholder on your local development machine, is extracting the archive with the HttPlaceholder binaries (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)). You need to put HttPlaceholder on your path variable yourself.

Alternatively, to install HttPlaceholder on Linux, run the following command in your terminal (make sure you're running as administrator):

```bash
curl -o- https://raw.githubusercontent.com/dukeofharen/httplaceholder/master/scripts/install-linux.sh | sudo bash
```

This is a self contained version of HttPlaceholder: no SDK has to be installed to run it.

If you would like to expose HttPlaceholder to the outside world, I would recommend to use Nginx or Apache as reverse proxy. To keep the service running even if you're not logged in through an SSH session, you can use something like systemd.

There are two Vagrant boxes (Windows and Ubuntu) that you can use to view how installation of HttPlaceholder is done. You can find them in this repository under the folder "vagrant". You need to have [Vagrant](https://www.vagrantup.com/) installed. After that, it's just a matter of going to the correct folder in your terminal and typing `vagrant up`. HttPlaceholder will then be installed under Windows or Ubuntu and can be reached by going to `http://localhost:8080` or `https://localhost:4430`.

## Mac

The only thing needed to use HttPlaceholder on your local development machine, is extracting the archive with the HttPlaceholder binaries (which can be found [here](https://github.com/dukeofharen/httplaceholder/releases/latest)). You need to put HttPlaceholder on your path variable yourself.

To install HttPlaceholder on Mac OS X, run the following command in your terminal (make sure you're running as administrator):

```bash
curl -o- https://raw.githubusercontent.com/dukeofharen/httplaceholder/master/scripts/install-mac.sh | sudo bash
```

This is a self contained version of HttPlaceholder: no SDK has to be installed to run it.

## Docker

HttPlaceholder has a Docker image; it can be found [here](https://hub.docker.com/r/dukeofharen/httplaceholder). This page explains some basics about the Docker image and some examples you can use.

### Basic example

This is a very basic example for running HttPlaceholder locally from the command line.

`docker run -p 5000:5000 dukeofharen/httplaceholder:latest`

HttPlaceholder can now be reached on `http://localhost:5000` (or `http://localhost:5000/ph-ui` to get to the management interface).

### Docker configuration

The Docker container uses the configuration values as specified [here](CONFIG.md). Here is an example of starting the HttPlaceholder container with different ports for HTTP and HTTPS:

`docker run -p 8080:8080 -p 4430:4430 --env port=8080 --env httpsPort=4430 dukeofharen/httplaceholder:latest`

### Docker Compose examples

In the links below, you'll find several hosting scenario's written for Docker Compose. If you want to run any of these examples on your PC, open your terminal, go to one of the folders that contain a `docker-compose.yml` example and run `docker-compose up`.

- [Hosting with file storage](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-file-storage)
- [Hosting with SQL Server](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-mssql)
- [Hosting with MySQL](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-mysql)
- [Hosting with SQLite](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-sqlite)
- [Hosting with .yml stub files](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-stub-files)

## Hosting

### Running behind reverse proxy

IIS, Nginx and Apache (and a lot of other web servers) have the option to run an application behind a reverse proxy. For HttPlaceholder to function correctly behind a reverse proxy, the server has to send a few "proxy" headers to HttPlaceholders. The following headers should be sent:

- `X-Forwarded-For`: contains all IP addresses of the calling client and all proxy servers in between the client and HttPlaceholder. Used to determine the IP of the calling client.
- `X-Forwarded-Proto`: contains the protocol of the original call to the proxying web server (`http` or `https`).
- `X-Forwarded-Host`: contains the hostname of the original call to the proxying web server(e.g. `httplaceholder.com`).

These headers are, right now, only used instead of the "real" values if the actual IP address of the proxy server is the loopback IP (e.g. `127.0.0.1`, `::1` etc.).

Read more about this subject for the specific web servers:
- [Nginx](https://www.nginx.com/resources/wiki/start/topics/examples/forwarded/)
- [Apache](https://httpd.apache.org/docs/2.4/mod/mod_proxy.html)
- [IIS](https://blogs.msdn.microsoft.com/webapps/2018/09/05/how-to-log-client-ip-when-iis-is-load-balanced-the-x-forwarded-for-header-xff/)

### Using SSL

HttPlaceholder supports HTTPS. See [configuration](#configuration) for more information on this. By default, it uses the private key that is installed with HttPlaceholder. This file is named `key.pfx` and the password is `1234`. Before using HttPlaceholder and calling the HTTPS URL, you'll need to make sure to import and trust the .pfx file on your OS. For your convenience, three scripts (for Windows, Linux and Mac) are added for installing and trusting the .pfx file of HttPlaceholder. You can find the script in the installation folder: `install-private-key.sh` for Mac and Linux and `Install-Private-Key.ps1` for Windows.

# Getting started

- Install HttPlaceholder (see [Installation](#installation)).
- Create a new .yaml file (e.g. `stub.yaml`).
- Copy and paste these contents in your new file:

```yml
- id: situation-01
  conditions:
    method: GET
    url:
      path: /users
      query:
        id: 12
        filter: first_name
  response:
    statusCode: 200
    text: |
      {
        "first_name": "John"
      }
    headers:
      Content-Type: application/json
```

- Open the terminal in the folder you've added the `stub.yaml` file and run the following command: `httplaceholder`.
  HttPlaceholder will now start and will load the stubs in the current folder.
  ![](img/httplaceholder_running.png)

- Perform a specific HTTP call to HttPlaceholder so your provided response will be returned.
  - For Linux / Mac (cURL needs to be installed): `curl "http://localhost:5000/users?id=12&filter=first_name" -D-`
  - For Windows (uses Powershell): `(Invoke-WebRequest "http://localhost:5000/users?id=12&filter=first_name").RawContent`

- You can view and inspect the performed requests in the user interface at <http://localhost:5000/ph-ui>.
  ![](img/request_in_ui.png)

For more sophisticated examples, go to the page [samples](#samples) to view samples for all supported HTTP condition checkers and response writers.

# Request conditions

Whenever HttPlaceholder receives a request, all the conditions of all stubs are checked to see which stub corresponds to the sent request. There are condition checkers for example the URL, posted data etc. This page explains more.

## General

Under the "conditions" element, you describe how the request should look like. If the incoming request matches the conditions, the [response](RESPONSE.md) will be returned.

```yml
- id: situation-03
  tenant: users-api
  conditions:
    method: GET
    url:
      path: /users
      query:
        id: 15
        filter: last_name
  response:
    statusCode: 200
    text: |
      {
        "last_name": "Jackson"
      }
    headers:
      Content-Type: application/json
```

This example uses both conditions. This means that when:
- The URL path contains "/users"
- A query string with name "id" and value "15" is sent.
- A query string with name "filter" and value "last_name" is sent.

If all these conditions match, the response as defined under the "response" element is returned. For more information about the response element, you can read more [here](RESPONSE.md).

The stub also has a "tenant" field defined. This is a free text field which is optional. This field makes it possible to do operations of multiple stubs at once (e.g. delete all stubs with a specific tenant, get all stubs of a specific tenant or update all stubs of a specific tenant). To learn more about tenants, go to [API](API.md).

## Description

A free text field where you can specify where the stub is for. It is optional.

```yml
- id: situation-01
  description: Returns something
  conditions:
    method: GET
    url:
      path: /users
  response:
    statusCode: 200
    text: OK
```

## Enabled

Describes whether the stub is enabled or not. If no `enabled` field is provided, the stub is enabled by default. Value can be `true` or `false`.

```yml
- id: is-disabled
  enabled: false
  conditions:
    method: GET
    url:
      path: /users
  response:
    text: This stub is disabled.
```

## Request scenario

Scenarios make it possible to make stubs stateful. When you assign a scenario to a stub, a hit counter will be kept for the scenario and it is also possible to assign a state to a scenario. The default state of a scenario is "Start". Right now, the scenario state is only kept in memory, which means that when the application is restarted, all the states will be reset.

The scenario state can be set either to response writers (see [response](RESPONSE.md)) or by calling the [API](API.md).

The scenario makes it possible to configure your stubs to return different responses on the same request.

```yaml
- id: scenario-test
  scenario: scenario-name
  conditions:
    url:
      path: /the-url
  response:
    text: OK!
```

### Hit counter checking

Whenever a stub that is attached to a scenario is hit, the hit counter for that scenario will be increased. This makes it possible to create stubs that check the hit counter of the scenario it is in. Here is an example:

```yaml
- id: min-hits
  scenario: min
  conditions:
    method: GET
    url:
      path: /min-hits
  response:
    text: OK, number of hits increased

- id: min-hits-clear
  scenario: min
  conditions:
    method: GET
    url:
      path: /min-hits
    scenario:
      minHits: 3
  response:
    text: OK, min hits reached. Clearing state.
    scenario:
      clearState: true
```

In this example, both stubs are part of the `min` scenario. Whenever the `/min-hits` URL is called, the hit counter of the scenario will be increased. Whenever the scenario has at least 3 hits, the `min-hits-clear` stub will be executed. The `clearState` response writer makes sure the scenario is reset (so the counter is reset to 0). For more information about that, click [here](RESPONSE.md).

Under the `conditions.scenario` option, you have 3 options for hit counter checking:

- `minHits`: the minimum number of (inclusive) hits a scenario should have been called.
- `maxHits`: the maximum number of (exclusive) hits a scenario should have been called.
- `exactHits`: the exact number of hits a scenario should have been called.

### State checking

A scenario can be in a specific state. A state is represented as a simple string value. Here is an example:

```yaml
- id: scenario-state-1
  scenario: scenario-state
  conditions:
    method: GET
    url:
      path: /state-check
    scenario:
      scenarioState: Start
  response:
    text: OK, scenario is in state 'Start'
    scenario:
      setScenarioState: state-2

- id: scenario-state-2
  scenario: scenario-state
  conditions:
    method: GET
    url:
      path: /state-check
    scenario:
      scenarioState: state-2
  response:
    text: OK, scenario is in state 'state-2'. Resetting to default.
    scenario:
      clearState: true
```

In this example, both stubs are part of the `scenario-state` scenario. Whenever the `/state-check` URL is called, HttPlaceholder will (in this case) check the current state (a fresh scenario state is always `Start`). If the stub is hit, the scenario state will be set to `state-2` by the `setScenarioState` response writer (see [response](RESPONSE.md)). Whenever the same URL is called again, the second stub will be hit and after that the scenario state will be reset to its default values.

## Priority

There are cases when a request matches multiple stub. If this is the case, you can use the "priority" element. With the priority element, you can specify which stub should be used if multiple stubs are found. The stub with the highest priority will be used. If you don't set the priority on the stub, it will be 0 by default.

```yml
- id: fallback
  priority: -1
  conditions: 
    method: GET
  response:
    statusCode: 200
    text: OK-Fallback

- id: situation-01
  conditions:
    method: GET
    url:
      path: /users
  response:
    statusCode: 200
    text: OK
```

In the scenario above, if you got to url `http://httplaceholder/users`, both stubs will be matched. Because the priority of the fallback stub is -1, the other stub will be used instead.

## URI

### Path

The path condition is used to check a part of the URL path (so the part after http://... and before the query string). The condition can both check on substring and regular expressions.

```yml
- id: situation-01
  conditions:
    method: GET
    url:
      path: /users
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/users/1

```yml
- id: situation-01
  conditions:
    method: GET
    url:
      # Now with regex. Path should exactly match /users in this case.
      path: ^/users$
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/users

### Full path

This condition checker looks a lot like the path checker, but this checker also checks extra URL parameters, like the query string. The condition can both check on substring and regular expressions.

```yml
- id: situation-01
  conditions:
    method: GET
    url:
      fullPath: /users?filter=first_name
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/users?filter=first_name

### Query string

This condition checker can check the query string in a name-value collection like way. The condition can both check on substring and regular expressions.

```yml
- id: situation-01
  conditions:
    method: GET
    url:
      query:
        id: 14
        filter: last_name
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath?id=14&filter=last_name

### Is HTTPS

This condition checker can be used to verify if a request uses HTTPS or not. To configure HttPlaceholder with HTTPS, read [configuration](CONFIG.md) (hint: it's not hard at all).

```yml
- id: ishttps-ok
  conditions:
    method: GET
    url:
      path: /ishttps-ok
      isHttps: true
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: https://localhost:5050/anyPath

## Method

This condition checker can check the HTTP method (e.g. GET, POST, PUT, DELETE etc.).

```yml
- id: situation-01
  conditions:
    method: GET
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath

## Security

### Basic authentication

This condition checker can check whether the sent basic authentication matches with the data in the stub.

```yml
- id: basic-auth
  conditions:
    method: GET
    basicAuthentication:
      username: user
      password: pass
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath
- Headers:
  - Authorization: Basic dXNlcjpwYXNz

## Request headers

This condition checker can check whether the sent headers match with the headers in the stub. The condition can both check on substring and regular expressions.

```yml
- id: basic-auth
  conditions:
    method: GET
    headers:
      X-Api-Key: secret123
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath
- Headers:
  - X-Api-Key: secret123

## Request body

### Raw body

This condition checker can check whether the posted body corresponds to the given rules in the stub. It is possible to add multiple conditions. The condition can both check on substring and regular expressions.

```yml
- id: situation-01
  conditions:
    method: POST
    url:
      path: \busers\b
    body:
      - \busername\b
      - \bjohn\b
  response:
    statusCode: 200
    text: '{"result": true}'
    headers:
      Content-Type: application/json
```

**Correct request**
- Method: POST
- URL: http://localhost:5000/users
- Headers:
  - Content-Type: application/x-www-form-urlencoded
- Body:
```json
{"username": "john"}
```

### Form

The form value condition checker can check whether the posted form values correspond to the given rules in the stub. It is possible to add multiple conditions. The condition can both check on substring and regular expressions.

```yml
- id: form-ok
  conditions:
    method: POST
    url:
      path: /form
    form:
      - key: key1
        value: sjaak
      - key: key2
        value: bob
      - key: key2
        value: ducoo
  response:
    text: OK
```

**Correct request**
- Method: POST
- URL: http://localhost:5000/form
- Body:
```
key1=sjaak&key2=bob&key2=ducoo
```

### JSON

The JSON condition checker can be used to check if the posted JSON is posted according to your specified conditions. You can specify both an array or an object as input for the condition. When checking for string values in a JSON property, HttPlaceholder will use regular expressions to check if the condition is OK.

```yml
- id: json-object
  conditions:
    method: POST
    json:
      username: ^username$
      subObject:
        strValue: stringInput
        boolValue: true
        doubleValue: 1.23
        dateTimeValue: 2021-04-16T21:23:03
        intValue: 3
        nullValue: null
        arrayValue:
          - val1
          - subKey1: subValue1
            subKey2: subValue2
  response:
    text: OK JSON OBJECT!
```

**Correct request**
- Method: POST
- URL: http://localhost:5000
- Body:
```json
{
  "username": "username",
  "subObject": {
    "strValue": "stringInput",
    "boolValue": true,
    "doubleValue": 1.23,
    "dateTimeValue": "2021-04-16T21:23:03",
    "intValue": 3,
    "nullValue": null,
    "arrayValue": [
      "val1",
      {
        "subKey1": "subValue1",
        "subKey2": "subValue2"
      }
    ]
  }
}
```

```yml
- id: json-array
  conditions:
    method: POST
    json:
      - val1
      - 3
      - 1.46
      - 2021-04-17T13:16:54
      - stringVal: val1
        intVal: 55
  response:
    text: OK JSON ARRAY!
```

**Correct request**
- Method: POST
- URL: http://localhost:5000
- Body:
```json
[
    "val1",
    3,
    1.46,
    "2021-04-17T13:16:54",
    {
        "stringVal": "val1",
        "intVal": 55
    }
]
```
### JSONPath

Using the JSONPath condition checker, you can check the posted JSON body to see if it contains the correct elements. It is possible to add multiple conditions.

**Using a string array**

```yml
- id: jpath-test
  conditions:
    method: PUT
    url:
      path: /users
    jsonPath:
      - "$.phoneNumbers[?(@.type=='iPhone')]"
  response:
    statusCode: 204
```

**Specifying the expected value separately**

The `expectedValue` variable of this condition can be used with regular expressions if needed.

```yml
- id: jpath-test
  conditions:This
    method: PUT
    url:
      path: /users
    jsonPath:
      - query: $.phoneNumbers[0].type
        expectedValue: iPhone
  response:
    statusCode: 204
```

**Specifying the expected value separately and a single JSONPath string**

Both JSONPath condition types can be combined.

```yml
- id: jpath-test
  conditions:
    method: PUT
    url:
      path: /users
    jsonPath:
      - $.name
      - query: $.phoneNumbers[0].type
        expectedValue: iPhone
  response:
    statusCode: 204
```

**Correct request**
- Method: PUT
- URL: http://localhost:5000/users
- Body:
```json
{
    "name": "John",
	"phoneNumbers": [{
		"type": "iPhone",
		"number": "0123-4567-8888"
	}, {
		"type": "home",
		"number": "0123-4567-8910"
	}]
}
```

### XPath

Using the XPath condition checker, you can check the posted XML body to see if it contains the correct elements. It is possible to add multiple conditions.

It is also possible to (pre)-set the XML namespaces of a posted XML body. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.

```yml
- id: regular-xml
  conditions:
    method: POST
    url:
      path: /thingy
    headers:
      Content-Type: application/soap+xml; charset=utf-8
    xpath:
      - queryString: /object/a[text() = 'TEST']
  response:
    statusCode: 200
    text: <result>OK</result>
    headers:
      Content-Type: text/xml
```

```yml
- id: regular-xml
  conditions:
    method: POST
    url:
      path: /thingy
    headers:
      Content-Type: application/soap+xml; charset=utf-8
    xpath:
      - queryString: /object/a[text() = 'TEST']
        namespaces:
          soap: http://www.w3.org/2003/05/soap-envelope
          m: http://www.example.org/stock/Reddy
  response:
    statusCode: 200
    text: <result>OK</result>
    headers:
      Content-Type: text/xml
```

**Correct request**
- Method: POST
- URL: http://localhost:5000/thingy
- Headers:
  - Content-Type: application/soap+xml; charset=utf-8
- Body:
```xml
<?xml version="1.0"?><object><a>TEST</a><b>TEST2</b></object>
```

## Client IP validation

It is also possible to set a condition to check the the client IP. A condition can be set for a single IP address or a whole IP range.

```yml
# Client IP address validation on a single IP address
- id: client-ip-1
  conditions:
    method: GET
    url:
      path: /client-ip-1
    clientIp: 127.0.0.1
  response:
    statusCode: 200
    text: OK
```

```yml
# Client IP address validation on an IP range
- id: client-ip-2
  conditions:
    method: GET
    url:
      path: /client-ip-2
    clientIp: '127.0.0.0/29'
  response:
    statusCode: 200
    text: OK
```

## Hostname

It is possible to check if a hostname in a request is correct. This condition can be used with regular expressions if needed.

```yml
# Check the hostname on full name.
- id: host-1
  conditions:
    method: GET
    host: httplaceholder.com
  response:
    statusCode: 200
    text: OK
```

```yml
- id: host-2
  conditions:
    method: GET
    host: http(.*)
  response:
    statusCode: 200
    text: OK
```