# Do request
PATCH {{rootUrl}}/http-method

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
jsonpath "$.stub.conditions.method" == "PATCH"

# Do request again
PATCH {{rootUrl}}/http-method

HTTP 200

[Asserts]
body == "OK!"