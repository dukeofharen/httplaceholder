# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: json-response-type
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /json-response-type
response:
  json: |
    {
        "key1": "val1"
    }
```

HTTP 200

# Check the request
GET {{rootUrl}}/json-response-type

HTTP 200
[Asserts]
jsonpath "$.key1" == "val1"
header "Content-Type" == "application/json"