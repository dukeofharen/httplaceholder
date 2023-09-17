# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: image-jpg
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /image.jpg
response:
  image:
    type: jpeg
    width: 1024
    height: 256
    backgroundColor: '#ffa0d3'
    text: Placeholder text that will be drawn in the image
    fontSize: 10
    jpegQuality: 95
    wordWrap: false
```

HTTP 200

# Check the request
GET {{rootUrl}}/image.jpg

HTTP 200
[Asserts]
bytes count > 0
header "Content-Type" == "image/jpeg"