# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: xml-response-type
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /xml-response-type
response:
  xml: <xml>value</xml>
```

HTTP 200

# Check the request
GET {{rootUrl}}/xml-response-type

HTTP 200
[Asserts]
body == "<xml>value</xml>"
header "Content-Type" == "text/xml"