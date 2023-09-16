# Create and test methods stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: multiple-methods
tenant: integration
conditions:
  method:
    - GET
    - POST
  url:
    path:
      equals: /multiple-methods
response:
  text: OK MULTIPLE METHODS
```

HTTP 200

# Check GET method
GET {{rootUrl}}/multiple-methods

HTTP 200
[Asserts]
body == "OK MULTIPLE METHODS"

# Check POST method
POST {{rootUrl}}/multiple-methods

HTTP 200
[Asserts]
body == "OK MULTIPLE METHODS"

# Check PUT method
PUT {{rootUrl}}/multiple-methods

HTTP 200
[Asserts]
body == "OK FALLBACK"