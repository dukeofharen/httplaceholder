# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: host-old-way
conditions:
  url:
    path: /host-old-way
  host: httplaceholder.local
response:
  text: host-old-way-ok
```

HTTP 200

# Check the request
GET {{rootUrl}}/host-old-way
Host: httplaceholder.local

HTTP 200
[Asserts]
body == "host-old-way-ok"

# Check the request: host incorrect
GET {{rootUrl}}/host-old-way
Host: stubs.local

HTTP 200
[Asserts]
body == "OK FALLBACK"