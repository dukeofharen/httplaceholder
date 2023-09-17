# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: slow-request
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /slow-request
response:
  extraDuration: 100
  text: OK SLOW REQUEST
```

HTTP 200

# Create stub with min/max
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: slow-request-model
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /slow-request-model
response:
  extraDuration:
    min: 100
    max: 160
  text: OK SLOW REQUEST MODEL
```

HTTP 200

# Check the request
GET {{rootUrl}}/slow-request

HTTP 200
[Asserts]
body == "OK SLOW REQUEST"
duration > 100

# Check the request: min/max
GET {{rootUrl}}/slow-request-model

HTTP 200
[Asserts]
body == "OK SLOW REQUEST MODEL"
duration > 100
duration < 300