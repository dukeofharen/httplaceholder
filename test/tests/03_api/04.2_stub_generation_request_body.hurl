# Do request
POST {{rootUrl}}/request-body
`Some request body`

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
jsonpath "$.stub.conditions.body[0].equals" == "Some request body"

# Do request again
POST {{rootUrl}}/request-body
`Some request body`

HTTP 200

[Asserts]
body == "OK!"