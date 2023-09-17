# Reset scenarios
DELETE {{rootUrl}}/ph-api/scenarios

# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-state-updating-1
scenario: state-updating
tenant: integration
conditions:
  method: GET
  scenario:
    scenarioState: Start
  url:
    path:
      equals: /state-updating
response:
  text: OK STATE IS UPDATED
  scenario:
    setScenarioState: NewState
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-state-updating-2
scenario: state-updating
tenant: integration
conditions:
  method: GET
  scenario:
    scenarioState: NewState
  url:
    path:
      equals: /state-updating
response:
  text: OK STATE IS CLEARED
  scenario:
    clearState: true
```

HTTP 200

# Check the request: state is updated
GET {{rootUrl}}/state-updating

HTTP 200
[Asserts]
body == "OK STATE IS UPDATED"

# Check the request: state is cleared
GET {{rootUrl}}/state-updating

HTTP 200
[Asserts]
body == "OK STATE IS CLEARED"

# Check the request: state is updated
GET {{rootUrl}}/state-updating

HTTP 200
[Asserts]
body == "OK STATE IS UPDATED"

# Check the request: state is cleared
GET {{rootUrl}}/state-updating

HTTP 200
[Asserts]
body == "OK STATE IS CLEARED"