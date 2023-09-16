# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: enable-disable-stub
enabled: true
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /enabled-or-disabled
response:
  text: OK STUB IS ENABLED
```

HTTP 200

# Check the request: stub is enabled
GET {{httpsRootUrl}}/enabled-or-disabled

HTTP 200
[Asserts]
body == "OK STUB IS ENABLED"

# Create stub: disable stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: enable-disable-stub
enabled: false
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /enabled-or-disabled
response:
  text: OK STUB IS ENABLED
```

HTTP 200

# Check the request: stub is disabled
GET {{httpsRootUrl}}/enabled-or-disabled

HTTP 200
[Asserts]
body == "OK FALLBACK"