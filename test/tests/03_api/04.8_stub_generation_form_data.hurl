# Do request
POST {{rootUrl}}/form-data
[FormParams]
key1: value1
key2: value2

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
jsonpath "$.stub.conditions.form[0].key" == "key1"
jsonpath "$.stub.conditions.form[0].value.equals" == "value1"
jsonpath "$.stub.conditions.form[1].key" == "key2"
jsonpath "$.stub.conditions.form[1].value.equals" == "value2"

# Do request again
POST {{rootUrl}}/form-data
[FormParams]
key1: value1
key2: value2

HTTP 200

[Asserts]
body == "OK!"