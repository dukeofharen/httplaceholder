# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: moved-permanently
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /moved-permanently
response:
  movedPermanently: https://ducode.org
```

HTTP 200

# Check the request
GET {{rootUrl}}/moved-permanently

HTTP 301
[Asserts]
header "Location" == "https://ducode.org"