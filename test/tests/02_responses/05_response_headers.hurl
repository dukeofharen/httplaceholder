# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: response-headers
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /response-headers
response:
  text: OK RESPONSE HEADERS
  headers:
    X-Header-1: val1
    X-Header-2: val2
```

HTTP 200

# Check the request
GET {{rootUrl}}/response-headers

HTTP 200
[Asserts]
body == "OK RESPONSE HEADERS"
header "X-Header-1" == "val1"
header "X-Header-2" == "val2"