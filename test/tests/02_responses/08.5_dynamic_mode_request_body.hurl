# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-request-body
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /dynamic-mode-request-body
response:
  enableDynamicMode: true
  headers:
    X-RequestBody: 'Header request body: ((request_body))'
  text: 'Request body: ((request_body))'
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-request-body-regex
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /dynamic-mode-request-body-regex
response:
  enableDynamicMode: true
  headers:
    X-RequestBody: "Header request body: ((request_body:'key2=([a-z0-9]*)'))"
  text: "Request body: ((request_body:'key3=([a-z0-9]*)'))"
```

HTTP 200

# Check the request
POST {{rootUrl}}/dynamic-mode-request-body
`the posted body`

HTTP 200
[Asserts]
body == "Request body: the posted body"
header "X-RequestBody" == "Header request body: the posted body"

# Check the request: regex
POST {{rootUrl}}/dynamic-mode-request-body-regex
```
key1=value1
key2=value2
key3=value3
```

HTTP 200
[Asserts]
body == "Request body: value3"
header "X-RequestBody" == "Header request body: value2"