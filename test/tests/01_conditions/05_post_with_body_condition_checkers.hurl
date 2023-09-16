# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: post-and-check-body
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post
  body:
    - contains: value1
    - contains: value2
response:
  text: OK POST AND CHECK BODY
```

HTTP 200

# Check the request
POST {{rootUrl}}/post
```
value1
value2
```

HTTP 200
[Asserts]
body equals "OK POST AND CHECK BODY"

# Check the request: posted value incorrect
POST {{rootUrl}}/post
```
value3
value2
```

HTTP 200
[Asserts]
body equals "OK FALLBACK"

# Check the request: method is incorrect
PUT {{rootUrl}}/post
```
value1
value2
```

HTTP 200
[Asserts]
body equals "OK FALLBACK"