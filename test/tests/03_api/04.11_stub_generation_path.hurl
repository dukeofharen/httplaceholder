# Do request
GET {{rootUrl}}/uri-path

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
jsonpath "$.stub.conditions.url.path.equals" == "/uri-path"

# Do request again
GET {{rootUrl}}/uri-path

HTTP 200

[Asserts]
body == "OK!"