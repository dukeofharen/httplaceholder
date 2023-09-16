# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: permanent-redirect
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /permanent-redirect
response:
  permanentRedirect: https://httplaceholder.org
```

HTTP 200

# Check the request
GET {{rootUrl}}/permanent-redirect

HTTP 308
[Asserts]
header "Location" == "https://httplaceholder.org"