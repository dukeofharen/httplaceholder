# Reset scenarios
DELETE {{rootUrl}}/ph-api/scenarios

# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-min-hits-only-get
scenario: min-hits
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /scenario-min-hits
response:
  text: OK HITS ARE INCREASED (MIN HITS)
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-min-hits
scenario: min-hits
tenant: integration
conditions:
  method: GET
  scenario:
    minHits: 2
  url:
    path:
      equals: /scenario-min-hits
response:
  text: OK MIN HITS IS REACHED
```

HTTP 200

# Check the request: min hit count not reached yet
GET {{rootUrl}}/scenario-min-hits

HTTP 200
[Asserts]
body == "OK HITS ARE INCREASED (MIN HITS)"

# Check the request: min hit count reached
GET {{rootUrl}}/scenario-min-hits

HTTP 200
[Asserts]
body == "OK MIN HITS IS REACHED"