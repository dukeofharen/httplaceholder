# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: html-response-type
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /html-response-type
response:
  html: <html>value</html>
```

HTTP 200

# Check the request
GET {{rootUrl}}/html-response-type

HTTP 200
[Asserts]
body == "<html>value</html>"
header "Content-Type" == "text/html"