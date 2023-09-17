# Do request
POST {{rootUrl}}/json-data
{
    "stringValue": "value",
    "intValue": 42.1,
    "boolValue": true,
    "objectValue": {
        "stringValue": "anotherValue"
    },
    "arrayValue": [
        "value1",
        333
    ]
}

HTTP 501

[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Create and check stub based on request
POST {{rootUrl}}/ph-api/requests/{{corr_id}}/stubs
{
    "doNotCreateStub": false
}

HTTP 200

[Asserts]
jsonpath "$.stub.conditions.json.stringValue" == "value"
jsonpath "$.stub.conditions.json.intValue" == 42.1
jsonpath "$.stub.conditions.json.boolValue" == true
jsonpath "$.stub.conditions.json.objectValue.stringValue" == "anotherValue"
jsonpath "$.stub.conditions.json.arrayValue" count == 2
jsonpath "$.stub.conditions.json.arrayValue[0]" == "value1"
jsonpath "$.stub.conditions.json.arrayValue[1]" == 333

# Do request again
POST {{rootUrl}}/json-data
{
    "stringValue": "value",
    "intValue": 42.1,
    "boolValue": true,
    "objectValue": {
        "stringValue": "anotherValue"
    },
    "arrayValue": [
        "value1",
        333
    ]
}

HTTP 200

[Asserts]
body == "OK!"