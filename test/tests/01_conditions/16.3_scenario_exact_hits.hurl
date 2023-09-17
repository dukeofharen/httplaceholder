# Reset scenarios
DELETE {{rootUrl}}/ph-api/scenarios

# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-exact-hits-only-get
scenario: exact-hits
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /scenario-exact-hits
response:
  text: OK EXACT HITS CONDITION NOT MET
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-exact-hits
scenario: exact-hits
tenant: integration
conditions:
  method: GET
  scenario:
    exactHits: 2
  url:
    path:
      equals: /scenario-exact-hits
response:
  text: OK EXACT HITS MET
```

HTTP 200

# Check the request: exact hit count not reached yet
GET {{rootUrl}}/scenario-exact-hits

HTTP 200
[Asserts]
body == "OK EXACT HITS CONDITION NOT MET"

# Check the request: exact hit count reached
GET {{rootUrl}}/scenario-exact-hits

HTTP 200
[Asserts]
body == "OK EXACT HITS MET"

# Check the request: exact hit count exceeded
GET {{rootUrl}}/scenario-exact-hits

HTTP 200
[Asserts]
body == "OK EXACT HITS CONDITION NOT MET"