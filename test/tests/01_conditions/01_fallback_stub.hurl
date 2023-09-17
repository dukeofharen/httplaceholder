# Create and test fallback stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: fallback
priority: -1
tenant: integration
response:
  text: OK FALLBACK
```

HTTP 200

# Check the request
GET {{rootUrl}}/urlthatisnotmapped

HTTP 200
[Asserts]
body == "OK FALLBACK"