# Binary request
POST {{rootUrl}}/binary
Content-Type: image/png
file,pixel.png;

HTTP 200
[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Get and check request
GET {{rootUrl}}/ph-api/requests/{{corr_id}}

HTTP 200
[Asserts]
jsonpath "$.requestParameters.bodyIsBinary" == true

# Text request
POST {{rootUrl}}/text
`Some text`

HTTP 200
[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Get and check request
GET {{rootUrl}}/ph-api/requests/{{corr_id}}

HTTP 200
[Asserts]
jsonpath "$.requestParameters.bodyIsBinary" == false