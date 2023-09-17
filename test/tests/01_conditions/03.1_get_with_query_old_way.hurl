# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-query-old-way
tenant: integration
conditions:
  method: GET
  url:
    path: /get-query-old-way
  query:
    q1: val1
    q2: val2
response:
  text: OK QUERY OLD WAY
```

HTTP 200

# Check the request
GET {{rootUrl}}/get-query-old-way?q1=val1&q2=val2

HTTP 200
[Asserts]
body == "OK QUERY OLD WAY"