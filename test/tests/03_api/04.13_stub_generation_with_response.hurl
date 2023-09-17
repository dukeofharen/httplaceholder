# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: stub-generation-with-response
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /stub-generation-with-response
response:
  text: OK STUB GENERATION WITH RESPONSE
```

HTTP 200

# Do request
GET {{rootUrl}}/stub-generation-with-response

HTTP 200

[Captures]
corr_id: header "X-HttPlaceholder-Correlation"

# Create stub based on request
POST {{rootUrl}}/ph-api/requests/{{corr_id}}/stubs
{
    "doNotCreateStub": false
}

HTTP 200

[Asserts]
jsonpath "$.stub.response.text" == "OK STUB GENERATION WITH RESPONSE"