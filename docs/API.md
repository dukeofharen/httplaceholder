# HttPlaceholder REST API

Like many other automation and development tools, HttPlaceholder has a REST API that you can use to automate the creation of stubs. By default, the stubs are stored in memory and are thereby cleared when HttPlaceholder restarts (in the future, multiple stub sources, like file system or database persistency, may be added). The REST API gives you access to two collections: the stubs collection and the requests collection (to see all requests that are made to HttPlaceholder).

The complete API (so all request and response objects) can be found on [SwaggerHub](https://app.swaggerhub.com/apis/dukeofharen/httplaceholder_api/v1). This page contains all endpoints, but only a few samples. Click [here](swagger.json) if you only want the swagger.json file. Using this swagger.json file, you can easily create a REST client for your favourite programming language (e.g. using a tool like [autorest](https://github.com/Azure/autorest))

## General

The REST API accepts both JSON and YAML strings (when doing a POST). If you want to post a YAML string, set the `Content-Type` header to `application/x-yaml`, if you want to post a JSON string, set the `Content-Type` header to `application/json`. If you do a request where you expect a textual response, set the `Accept` header to `application/x-yaml` if you want to get YAML or `application/json` if you want to get JSON.

If you have enabled authentication (see [CMD.md](CMD.md) for more information), you also need to provide an `Authorization` header with the correct basic authentication. So if, for example, the username is `user` and the password is `pass`, the following value should be used for the `Authorization` header: `dXNlcjpwYXNz`. For every call in the REST API, a `401 Unauthorized` is returned if the authentication is incorrect.

## Stubs

### (POST) Add stub

Adds a new stub.

**Request URL**<br />
`http://httplaceholder/ph-api/stubs`

**Request body**
```
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
```
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

### (GET) Get stub

Returns a specific stub by its ID.

**Request URL**<br />
`http://httplaceholder/ph-api/stubs/{stubId}`

- `stubId`: the ID of the stub to retrieve (e.g. `situation-01`)

**Request headers**
- `Accept`: `application/x-yaml`

**Response**<br />
`200 OK`
```
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
```
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
```
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