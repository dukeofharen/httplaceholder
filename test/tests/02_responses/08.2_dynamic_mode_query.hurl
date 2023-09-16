# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-query-string
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-query-string
response:
  enableDynamicMode: true
  headers:
    X-QueryString: 'Query string header: ((query:q1))'
  text: 'Query string: ((query:q2))'
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-query-string?q1=val1&q2=val2

HTTP 200
[Asserts]
body == "Query string: val2"
header "X-QueryString" == "Query string header: val1"