# Reset scenarios
DELETE {{rootUrl}}/ph-api/scenarios
HTTP 204

DELETE {{rootUrl}}/ph-api/scenarios
X-HttPlaceholder-DistKey: dist1
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

# Get scenario for another dist. key: not found
GET {{rootUrl}}/ph-api/scenarios/scenario-1
X-HttPlaceholder-DistKey: dist1

HTTP 404

# Set scenario for another dist. key
PUT {{rootUrl}}/ph-api/scenarios/scenario-1
X-HttPlaceholder-DistKey: dist1
{
    "state": "another-new-state",
    "hitCount": 11
}
HTTP 204

# Get scenario for another dist. key: found
GET {{rootUrl}}/ph-api/scenarios/scenario-1
X-HttPlaceholder-DistKey: dist1

HTTP 200

[Asserts]
jsonpath "$.scenario" == "scenario-1"
jsonpath "$.state" == "another-new-state"
jsonpath "$.hitCount" == 11