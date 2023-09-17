# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: temporary-redirect
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /temp-redirect
response:
  temporaryRedirect: https://google.com
```

HTTP 200

# Check the request
GET {{rootUrl}}/temp-redirect

HTTP 307
[Asserts]
header "Location" == "https://google.com"