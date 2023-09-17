# Do request
POST {{rootUrl}}/json-data
[
    "value1",
    44,
    false,
    {
        "key1": "val1"
    },
    [
        "1",
        2
    ]
]

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
jsonpath "$.stub.conditions.json[0]" == "value1"
jsonpath "$.stub.conditions.json[1]" == 44
jsonpath "$.stub.conditions.json[2]" == false
jsonpath "$.stub.conditions.json[3].key1" == "val1"
jsonpath "$.stub.conditions.json[4]" count == 2
jsonpath "$.stub.conditions.json[4][0]" == "1"
jsonpath "$.stub.conditions.json[4][1]" == 2

# Do request again
POST {{rootUrl}}/json-data
[
    "value1",
    44,
    false,
    {
        "key1": "val1"
    },
    [
        "1",
        2
    ]
]

HTTP 200

[Asserts]
body == "OK!"