# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-path-old-way
tenant: integration
conditions:
  method: GET
  url:
    path: /get-path-old-way
response:
  text: OK PATH OLD WAY
```

HTTP 200

# Check the request
GET {{rootUrl}}/get-path-old-way

HTTP 200
[Asserts]
body == "OK PATH OLD WAY"