# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-full-path-old-way
tenant: integration
conditions:
  method: GET
  url:
    fullPath: /get-full-path-old-way?q1=v1&q2=v2
response:
  text: OK FULL PATH OLD WAY
```

HTTP 200

# Check the request
GET {{rootUrl}}/get-full-path-old-way?q1=v1&q2=v2

HTTP 200
[Asserts]
body == "OK FULL PATH OLD WAY"