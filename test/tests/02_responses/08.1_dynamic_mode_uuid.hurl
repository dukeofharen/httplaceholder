# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-uuid
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-uuid
response:
  enableDynamicMode: true
  headers:
    X-Uuid: ((uuid))
  text: ((uuid))
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-uuid

HTTP 200
[Asserts]
body matches /\b[0-9a-f]{8}\b-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-\b[0-9a-f]{12}\b/
header "X-Uuid" matches /\b[0-9a-f]{8}\b-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-\b[0-9a-f]{12}\b/