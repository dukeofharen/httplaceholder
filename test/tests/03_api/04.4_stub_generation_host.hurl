# Do request
GET {{rootUrl}}/host
Host: httplaceholder.com

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
jsonpath "$.stub.conditions.host.equals" == "httplaceholder.com"

# Do request again
GET {{rootUrl}}/host
Host: httplaceholder.com

HTTP 200

[Asserts]
body == "OK!"