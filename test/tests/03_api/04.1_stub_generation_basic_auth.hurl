# Do request
GET {{rootUrl}}/basic-auth
[BasicAuth]
test-user: test-pass

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
jsonpath "$.stub.conditions.basicAuthentication.username" == "test-user"
jsonpath "$.stub.conditions.basicAuthentication.password" == "test-pass"

# Do request again
GET {{rootUrl}}/basic-auth
[BasicAuth]
test-user: test-pass

HTTP 200

[Asserts]
body == "OK!"