# Do request
GET {{rootUrl}}/some-request

HTTP 200

[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Delete request: OK
DELETE {{rootUrl}}/ph-api/requests/{{corr_id}}
HTTP 204

# Delete request: not found
DELETE {{rootUrl}}/ph-api/requests/{{corr_id}}
HTTP 404