# Reset scenarios
DELETE {{rootUrl}}/ph-api/scenarios
HTTP 204

# Get scenario: not found
GET {{rootUrl}}/ph-api/scenarios/scenario-1
HTTP 404

# Set scenario
PUT {{rootUrl}}/ph-api/scenarios/scenario-1
{
    "state": "new-state",
    "hitCount": 10
}
HTTP 204

# Get scenario: found
GET {{rootUrl}}/ph-api/scenarios/scenario-1
HTTP 200

[Asserts]
jsonpath "$.scenario" == "scenario-1"
jsonpath "$.state" == "new-state"
jsonpath "$.hitCount" == 10

# Get all scenarios: 1 result
GET {{rootUrl}}/ph-api/scenarios
HTTP 200

[Asserts]
jsonpath "$" count == 1
jsonpath "$[0].scenario" == "scenario-1"

# Delete scenario
DELETE {{rootUrl}}/ph-api/scenarios/scenario-1
HTTP 204

# Delete scenario: not found
DELETE {{rootUrl}}/ph-api/scenarios/scenario-1
HTTP 404

# Get all scenarios: no results
GET {{rootUrl}}/ph-api/scenarios
HTTP 200

[Asserts]
jsonpath "$" count == 0