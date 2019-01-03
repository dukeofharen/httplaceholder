# HttPlaceholder REST API

Like many other automation and development tools, HttPlaceholder has a REST API that you can use to automate the creation of stubs. By default, the stubs are stored in memory and are thereby cleared when HttPlaceholder restarts (you can persist the stubs and request; see [config](CONFIG.md)). The REST API gives you access to four collections: the stubs collection, the requests collection (to see all requests that are made to HttPlaceholder), users collection and tenants collection.

Click [here](https://github.com/dukeofharen/httplaceholder/releases/latest) if you want the swagger.json file. Using this swagger.json file, you can easily create a REST client for your favourite programming language (e.g. using a tool like [autorest](https://github.com/Azure/autorest))

## General

The REST API accepts both JSON and YAML strings (when doing a POST or PUT). If you want to post a YAML string, set the `Content-Type` header to `application/x-yaml`, if you want to post a JSON string, set the `Content-Type` header to `application/json`. If you do a request where you expect a textual response, set the `Accept` header to `application/x-yaml` if you want to get YAML or `application/json` if you want to get JSON.

If you have enabled authentication (see [CONFIG.md](CONFIG.md) for more information), you also need to provide an `Authorization` header with the correct basic authentication. So if, for example, the username is `user` and the password is `pass`, the following value should be used for the `Authorization` header: `Basic dXNlcjpwYXNz`. For every call in the REST API, a `401 Unauthorized` is returned if the authentication is incorrect.

## Stubs

### (POST) Add stub

Adds a new stub.

**Request URL**<br />
`http://httplaceholder/ph-api/stubs`

**Request body**
```yml
id: situation-01
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
      ""first_name"": ""John""
    }
  headers:
    Content-Type: application/json
```

**Request headers**
- `Content-Type`: `application/x-yaml`

**Response**<br />
`204 NoContent`

### (GET) Get all stubs

Returns a list with all configured stubs.

**Request URL**<br />
`http://httplaceholder/ph-api/stubs`

**Request headers**
- `Accept`: `application/x-yaml`

**Response**<br />
`200 OK`
```yml
- stub:
    id: situation-01
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
          ""first_name"": ""John""
        }
      headers:
        Content-Type: application/json
  metadata:
    readOnly: true

- stub:
    id: situation-02
    conditions:
      method: POST
      url:
        path: /users
        query:
          id: 12
          filter: first_name
    response:
      statusCode: 200
      text: |
        {
          ""first_name"": ""John""
        }
      headers:
        Content-Type: application/json
  metadata:
    readOnly: true
```

### (GET) Get stub

Returns a specific stub by its ID.

**Request URL**<br />
`http://httplaceholder/ph-api/stubs/{stubId}`

- `stubId`: the ID of the stub to retrieve (e.g. `situation-01`)

**Request headers**
- `Accept`: `application/x-yaml`

**Response**<br />
`200 OK`
```yml
stub:
  id: situation-01
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
        ""first_name"": ""John""
      }
    headers:
      Content-Type: application/json
metadata:
  readOnly: true
```

`404 NotFound`

### (DELETE) Delete stub

Deletes a specific stub by its ID.

**Request URL**<br />
`http://httplaceholder/ph-api/stubs/{stubId}`

- `stubId`: the ID of the stub to delete (e.g. `situation-01`)

**Request headers**
- `Accept`: `application/x-yaml`

**Response**<br />
`204 NoContent`

`404 NotFound`

## Requests

### (GET) Get all performed requests

Returns a list with all performed HTTP requests to HttPlaceholder.

**Request URL**<br />
`http://httplaceholder/ph-api/requests`

**Request headers**
- `Accept`: `application/json`

**Response**<br />
`200 OK`
```json
[{
	"correlationId": "20421e53-dde2-4dbb-8565-afb0b34ebd83",
	"requestParameters": null,
	"logLines": [],
	"stubExecutionResult": [],
	"executingStubId": null,
	"requestBeginTime": "0001-01-01T00:00:00",
	"requestEndTime": "0001-01-01T00:00:00"
}]
```

### (GET) Get all performed requests by stub ID

Returns a list with all performed HTTP requests to HttPlaceholder by a specific stub ID.

**Request URL**<br />
`http://httplaceholder/ph-api/requests/{stubId}`

- `stubId`: the ID of the stub to delete (e.g. `stub1`)

**Request headers**
- `Accept`: `application/json`

**Response**<br />
`200 OK`
```json
[{
	"correlationId": null,
	"requestParameters": null,
	"logLines": [],
	"stubExecutionResult": [],
	"executingStubId": "stub1",
	"requestBeginTime": "0001-01-01T00:00:00",
	"requestEndTime": "0001-01-01T00:00:00"
}]
```

## Users

### (GET) Logged in user

Returns the given username.

Note: while this method might look unnecessary at first sight, it can be used to check if an instance of HttPlaceholder has basic authentication configured.

**Request URL**<br />
`http://httplaceholder/ph-api/users/{username}`

- `username`: the username to check

**Request headers**
- `Accept`: `application/json`

**Response**<br />
`200 OK`
```json
{
    "username": "username"
}
```

`403 Forbidden` (if username in basic authentication and URL don't match)

## Tenants

Tenants allow you to group your stubs in groups. When you've assigned a "tenant" field to your stub (see [conditions](CONDITIONS.md) for more information), you can perform batch operations on a larger set of stubs. The tenants endpoint helps you with this.

### (GET) Get all stubs in a tenant

Returns all stubs in a given tenant.

**Request URL**<br />
`http://httplaceholder/ph-api/tenants/{tenant}/stubs`

- `tenant`: the tenant for which the stubs should be returned

**Request headers**
- `Accept`: `application/json`

**Response**<br />
`200 OK`
```json
[
    {
      "stub": {
        "id": "regular-xml",
        "conditions": {
            "method": "POST",
            "url": {
                "path": "/thingy"
            },
            "headers": {
                "Content-Type": "application/soap+xml; charset=utf-8"
            },
            "xpath": [
                {
                    "queryString": "/object/a[text() = 'TEST']"
                }
            ]
        },
        "response": {
            "statusCode": 200,
            "text": "<result>OK</result>",
            "headers": {
                "Content-Type": "text/xml"
            }
        },
        "priority": 1,
        "tenant": "03-xml"
      },
      "metadata": {
        "readOnly": true
      }
    },
    {
      "stub": {
        "id": "soap-xml",
        "conditions": {
            "method": "POST",
            "url": {
                "path": "/InStock"
            },
            "headers": {
                "Content-Type": "application/soap+xml; charset=utf-8"
            },
            "xpath": [
                {
                    "queryString": "/soap:Envelope/soap:Body/m:GetStockPrice/m:StockName[text() = 'GOOG']",
                    "namespaces": {
                        "soap": "http://www.w3.org/2003/05/soap-envelope",
                        "m": "http://www.example.org/stock/Reddy"
                    }
                }
            ]
        },
        "response": {
            "statusCode": 200,
            "text": "<result>OK</result>",
            "headers": {
                "Content-Type": "text/xml"
            }
        },
        "priority": 1,
        "tenant": "03-xml"
      },
      "metadata": {
        "readOnly": true
      }
    }
]
```

### (DELETE) Delete all stubs from a tenant

Deletes all stubs in a given tenant.

**Request URL**<br />
`http://httplaceholder/ph-api/tenants/{tenant}/stubs`

- `tenant`: the tenant for which the stubs should be returned

**Response**
`204 NoContent`

### (PUT) Update all stubs in a tenant

Update all stubs for a given tenant. Stubs that are in the request will be added and stubs already present in HttPlaceholder under the same tenant, but not in the request, will be deleted. In this sense, this is a "make it so" request.

**Request URL**<br />
`http://httplaceholder/ph-api/tenants/{tenant}/stubs`

- `tenant`: the tenant for which the stubs should be returned

**Request headers**
- `Accept`: `text/yaml`

**Request body**
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
        ""first_name"": ""John""
      }
    headers:
      Content-Type: application/json

- id: situation-02
  conditions:
    method: POST
    url:
      path: /users
      query:
        id: 12
        filter: first_name
  response:
    statusCode: 200
    text: |
      {
        ""first_name"": ""John""
      }
    headers:
      Content-Type: application/json
```

**Response**
`204 NoContent`