# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: windows-line-endings
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /windows-line-endings
response:
  lineEndings: windows
  text: |
    text
    with
    windows
    line
    endings
```

HTTP 200

# Check the request
GET {{rootUrl}}/windows-line-endings

HTTP 200
[Asserts]
body == "text\r\nwith\r\nwindows\r\nline\r\nendings\r\n"