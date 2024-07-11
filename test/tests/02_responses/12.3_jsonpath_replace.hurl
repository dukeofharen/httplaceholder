# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: jsonpath-replace
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /jsonpath-replace
response:
  json: '{"name": "Bassie", "city": "Amsterdam"}'
  replace:
    - jsonPath: '$.name'
      replaceWith: 'Adriaan'
    - jsonPath: '$.city'
      replaceWith: 'Groningen'
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: jsonpath-replace-dynamic
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /jsonpath-replace-dynamic
response:
  enableDynamicMode: true
  json: '{"name": "Bassie", "city": "Amsterdam"}'
  replace:
    - jsonPath: '$.name'
      replaceWith: ((query:q1))
    - jsonPath: '$.city'
      replaceWith: ((query:q2))
```

HTTP 200

# Check the request: JSONPath replace
GET {{rootUrl}}/jsonpath-replace

HTTP 200
[Asserts]
jsonpath "$.name" == "Adriaan"
jsonpath "$.city" == "Groningen"

# Check the request: regex replace with dynamic mode
GET {{rootUrl}}/jsonpath-replace-dynamic?q1=Pipo&q2=Assen

HTTP 200
[Asserts]
jsonpath "$.name" == "Pipo"
jsonpath "$.city" == "Assen"
