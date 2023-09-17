# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: status-code
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /status-code
response:
  statusCode: 401
  text: UNAUTHORIZED!
```

HTTP 200

# Check the request
GET {{rootUrl}}/status-code

HTTP 401
[Asserts]
body == "UNAUTHORIZED!"