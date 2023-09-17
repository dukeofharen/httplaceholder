# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: unix-line-endings
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /unix-line-endings
response:
  lineEndings: unix
  text: |
    text
    with
    unix
    line
    endings
```

HTTP 200

# Check the request
GET {{rootUrl}}/unix-line-endings

HTTP 200
[Asserts]
body == "text\nwith\nunix\nline\nendings\n"