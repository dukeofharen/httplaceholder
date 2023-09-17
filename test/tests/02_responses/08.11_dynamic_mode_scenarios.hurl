# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-scenario-state
scenario: dynamic-mode-scenario-state
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-scenario-state
response:
  enableDynamicMode: true
  headers:
    X-Value: '((scenario_state)) ((scenario_state:scenario123))'
  text: '((scenario_state)) ((scenario_state:scenario123))'
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-scenario-hitcount
scenario: dynamic-mode-scenario-hitcount
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-scenario-hitcount
response:
  enableDynamicMode: true
  headers:
    X-Value: '((scenario_hitcount)) ((scenario_hitcount:scenario123))'
  text: '((scenario_hitcount)) ((scenario_hitcount:scenario123))'
```

HTTP 200

# Set scenarios
PUT {{rootUrl}}/ph-api/scenarios/dynamic-mode-scenario-state
{
    "state": "cool_state_1",
    "hitCount": 10
}

HTTP 204

PUT {{rootUrl}}/ph-api/scenarios/dynamic-mode-scenario-hitcount
{
    "state": "cool_state_111",
    "hitCount": 4
}

HTTP 204

PUT {{rootUrl}}/ph-api/scenarios/scenario123
{
    "state": "cool_state_2",
    "hitCount": 10
}

HTTP 204

# Check the request: scenario state
GET {{rootUrl}}/dynamic-mode-scenario-state

HTTP 200
[Asserts]
body == "cool_state_1 cool_state_2"
header "X-Value" == "cool_state_1 cool_state_2"

# Check the request: hit count
GET {{rootUrl}}/dynamic-mode-scenario-hitcount

HTTP 200
[Asserts]
body == "5 10"
header "X-Value" == "5 10"