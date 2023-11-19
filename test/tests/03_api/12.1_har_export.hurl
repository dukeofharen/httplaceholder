# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-with-full-path
tenant: integration
conditions:
  method: GET
  url:
    fullPath:
      equals: /fullPath?q1=val1&q2=val2
response:
  text: OK FULL PATH
```

HTTP 200

# Perform request
GET {{rootUrl}}/fullPath?q1=val1&q2=val2

HTTP 200

[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Export as cURL
GET {{rootUrl}}/ph-api/export/requests/{{corr_id}}?type=Har

HTTP 200

[Asserts]
jsonpath "$.result" contains "fullPath?q1=val1&q2=val2"
jsonpath "$.requestExportType" == "Har"