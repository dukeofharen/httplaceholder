# Perform request
GET {{rootUrl}}/some-url?q1=val1&q2=val2

HTTP 501

[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Export as cURL
GET {{rootUrl}}/ph-api/export/requests/{{corr_id}}?type=Curl

HTTP 200

[Asserts]
jsonpath "$.result" == "curl 'http://localhost:5000/some-url?q1=val1&q2=val2' -H 'Accept: */*' -H 'Host: localhost:5000' -H 'User-Agent: hurl/4.3.0'"
jsonpath "$.requestExportType" == "Curl"