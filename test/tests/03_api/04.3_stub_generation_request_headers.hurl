# Do request
GET {{rootUrl}}/request-body
X-Header-1: value1
X-Header-2: value2

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
jsonpath "$.stub.conditions.headers['X-Header-1'].equals" == "value1"
jsonpath "$.stub.conditions.headers['X-Header-2'].equals" == "value2"

# Do request again
GET {{rootUrl}}/request-body
X-Header-1: value1
X-Header-2: value2

HTTP 200

[Asserts]
body == "OK!"