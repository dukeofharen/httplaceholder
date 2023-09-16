# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: post-and-check-body-old-way
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-and-check-body-old-way
  body:
    - contains: value1
    - contains: value2
response:
  text: OK POST AND CHECK BODY OLD WAY
```

HTTP 200

# Check the request
POST {{rootUrl}}/post-and-check-body-old-way
```
value1
value2
```

HTTP 200
[Asserts]
body == "OK POST AND CHECK BODY OLD WAY"