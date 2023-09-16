# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-with-full-path
tenant: integration
conditions:
  method: GET
  url:
    fullPath:
      equals: /fullPath?q1=val1&q2=val2
response:
  text: OK FULL PATH
```

HTTP 200

# Check the request
GET {{rootUrl}}/fullPath?q1=val1&q2=val2

HTTP 200
[Asserts]
body equals "OK FULL PATH"

# Check the request: path incorrect
GET {{rootUrl}}/fullPath?q1=val1&q2=val3

HTTP 200
[Asserts]
body equals "OK FALLBACK"