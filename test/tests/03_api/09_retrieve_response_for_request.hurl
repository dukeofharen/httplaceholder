# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: stub-for-get-response
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /get-response
response:
  text: OK RESPONSE RETRIEVED
  headers:
    X-Header-1: header1
```

HTTP 200

# Perform request
GET {{rootUrl}}/get-response
HTTP 200
[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Check response
GET {{rootUrl}}/ph-api/requests/{{corr_id}}/response

HTTP 200

[Asserts]
jsonpath "$.statusCode" == 200
jsonpath "$.body" == "T0sgUkVTUE9OU0UgUkVUUklFVkVE"
jsonpath "$.bodyIsBinary" == false
jsonpath "$.headers['X-Header-1']" == "header1"
jsonpath "$.headers['Content-Type']" == "text/plain"