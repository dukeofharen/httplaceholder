# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: text-response-type
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /text-response-type
response:
  text: OK TEXT RESPONSE
```

HTTP 200

# Check the request
GET {{rootUrl}}/text-response-type

HTTP 200
[Asserts]
body == "OK TEXT RESPONSE"
header "Content-Type" == "text/plain"