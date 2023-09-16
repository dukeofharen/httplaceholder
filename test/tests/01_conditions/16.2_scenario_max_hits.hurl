# Reset scenarios
DELETE {{rootUrl}}/ph-api/scenarios

# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-max-hits-only-get
scenario: max-hits
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /scenario-max-hits
response:
  text: OK MAX HITS EXCEEDED
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-max-hits
scenario: max-hits
tenant: integration
conditions:
  method: GET
  scenario:
    maxHits: 2
  url:
    path:
      equals: /scenario-max-hits
response:
  text: OK MAX HITS
```

HTTP 200

# Check the request: max hit count not reached yet
GET {{rootUrl}}/scenario-max-hits

HTTP 200
[Asserts]
body == "OK MAX HITS"

# Check the request: max hit count reached
GET {{rootUrl}}/scenario-max-hits

HTTP 200
[Asserts]
body == "OK MAX HITS EXCEEDED"