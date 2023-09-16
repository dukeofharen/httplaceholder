# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: regex-replace
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /regex-replace
response:
  text: Lorem ipsum dolor sit amet, consectetur adipiscing elit.
  replace:
    - regex: '(ipsum|consectetur)'
      replaceWith: Bassie
    - regex: '(amet|elit)'
      replaceWith: Adriaan
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: regex-replace-dynamic
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /regex-replace-dynamic
response:
  enableDynamicMode: true
  text: Lorem ipsum dolor sit amet, consectetur adipiscing elit.
  replace:
    - regex: '(ipsum|consectetur)'
      replaceWith: ((query:q1))
    - regex: '(amet|elit)'
      replaceWith: ((query:q2))
```

HTTP 200

# Check the request: regex replace
GET {{rootUrl}}/regex-replace

HTTP 200
[Asserts]
body == "Lorem Bassie dolor sit Adriaan, Bassie adipiscing Adriaan."

# Check the request: regex replace with dynamic mode
GET {{rootUrl}}/regex-replace-dynamic?q1=val1&q2=val2

HTTP 200
[Asserts]
body == "Lorem val1 dolor sit val2, val1 adipiscing val2."