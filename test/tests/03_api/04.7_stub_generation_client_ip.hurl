# Do request
PATCH {{rootUrl}}/client-ip
X-Forwarded-For: 1.1.1.1

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
jsonpath "$.stub.conditions.clientIp" == "1.1.1.1"

# Do request again
PATCH {{rootUrl}}/client-ip
X-Forwarded-For: 1.1.1.1

HTTP 200

[Asserts]
body == "OK!"