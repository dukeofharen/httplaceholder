# Do request
GET {{rootUrl}}/query-params?key1=val1&key2=val2

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
jsonpath "$.stub.conditions.url.query.key1.equals" == "val1"
jsonpath "$.stub.conditions.url.query.key2.equals" == "val2"

# Do request again
GET {{rootUrl}}/query-params?key1=val1&key2=val2

HTTP 200

[Asserts]
body == "OK!"