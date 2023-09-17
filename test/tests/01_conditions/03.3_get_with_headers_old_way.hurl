# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-headers-old-way
tenant: integration
conditions:
  method: GET
  url:
    path: /get-headers-old-way
  headers:
    Header-1: val1
    Header-2: val2
response:
  text: OK HEADERS OLD WAY
```

HTTP 200

# Check the request
GET {{rootUrl}}/get-headers-old-way
Header-1: val1
Header-2: val2

HTTP 200
[Asserts]
body == "OK HEADERS OLD WAY"