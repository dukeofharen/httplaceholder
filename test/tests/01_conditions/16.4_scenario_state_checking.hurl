# Reset scenarios
DELETE {{rootUrl}}/ph-api/scenarios

# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-state-checking-1
scenario: state-checking
tenant: integration
conditions:
  method: GET
  scenario:
    scenarioState: Start
  url:
    path:
      equals: /state-checking
response:
  text: OK STATE IS STARTED
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: scenario-state-checking-2
scenario: state-checking
tenant: integration
conditions:
  method: GET
  scenario:
    scenarioState: state-1
  url:
    path:
      equals: /state-checking
response:
  text: OK STATE IS STATE 1
```

HTTP 200

# Check the request: first state
GET {{rootUrl}}/state-checking

HTTP 200
[Asserts]
body == "OK STATE IS STARTED"

# Set scenario to another state
PUT {{rootUrl}}/ph-api/scenarios/state-checking
{
    "state": "state-1"
}

HTTP 204

# Check the request: second state
GET {{rootUrl}}/state-checking

HTTP 200
[Asserts]
body == "OK STATE IS STATE 1"