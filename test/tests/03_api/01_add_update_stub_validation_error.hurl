# Add stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /bla
response:
  statusCode: 600
```

HTTP 400

[Asserts]
jsonpath "$[0]" == "The Id field is required."
jsonpath "$[1]" == "Field 'StatusCode' should be between '100' and '599'."

# Update
PUT {{rootUrl}}/ph-api/stubs/stub-1
Content-Type: text/yaml
```
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /bla
response:
  statusCode: 600
```

HTTP 400

[Asserts]
jsonpath "$[0]" == "The Id field is required."
jsonpath "$[1]" == "Field 'StatusCode' should be between '100' and '599'."