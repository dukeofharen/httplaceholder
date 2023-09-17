# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-request-headers
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-request-headers
response:
  enableDynamicMode: true
  headers:
    X-RequestHeader: 'Request header in header: ((request_header:X-Header-1))'
  text: 'Request header: ((request_header:X-Header-2))'
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-request-headers
X-Header-1: val1
X-Header-2: val2

HTTP 200
[Asserts]
body == "Request header: val2"
header "X-RequestHeader" == "Request header in header: val1"