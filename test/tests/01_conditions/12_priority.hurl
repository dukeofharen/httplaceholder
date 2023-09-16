# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: priority-1
tenant: integration
priority: 1
conditions:
  url:
    path:
      equals: /priority
  method: GET
response:
  text: OK PRIO 1
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: priority-2
tenant: integration
priority: 2
conditions:
  url:
    path:
      equals: /priority
  method: GET
response:
  text: OK PRIO 2
```

HTTP 200

# Check the request
GET {{rootUrl}}/priority
Host: httplaceholder.local

HTTP 200
[Asserts]
body == "OK PRIO 2"