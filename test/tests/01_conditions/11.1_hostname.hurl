# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-with-hostname
tenant: integration
conditions:
  url:
    path:
      equals: /get-with-hostname
  method: GET
  host:
    equals: httplaceholder.local
response:
  text: OK HOSTNAME
```

HTTP 200

# Check the request
GET {{rootUrl}}/get-with-hostname
Host: httplaceholder.local

HTTP 200
[Asserts]
body == "OK HOSTNAME"

# Check the request: hostname incorrect
GET {{rootUrl}}/get-with-hostname
Host: stubs.local

HTTP 200
[Asserts]
body == "OK FALLBACK"