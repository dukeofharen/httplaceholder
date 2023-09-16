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
  extraDuration: 1000
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
    min: 1000
    max: 1500
  text: OK SLOW REQUEST MODEL
```

HTTP 200

# Check the request
GET {{rootUrl}}/slow-request

HTTP 200
[Asserts]
body == "OK SLOW REQUEST"
duration > 1000

# Check the request: min/max
GET {{rootUrl}}/slow-request-model

HTTP 200
[Asserts]
body == "OK SLOW REQUEST MODEL"
duration > 1000
duration < 1600