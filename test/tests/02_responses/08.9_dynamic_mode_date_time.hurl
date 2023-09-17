# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-date-time
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-date-time
response:
  enableDynamicMode: true
  headers:
    X-DateTime: ((utcnow:yyyy-MM-dd HH:mm:ss))
  text: ((localnow:yyyy-MM-dd HH:mm:ss))
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-date-time

HTTP 200
[Asserts]
body matches /\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}/
header "X-DateTime" matches /\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}/