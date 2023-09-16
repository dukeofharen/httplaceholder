# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-with-https
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /ishttps
    isHttps: true
response:
  text: OK HTTPS
```

HTTP 200

# Check the request
GET {{httpsRootUrl}}/ishttps

HTTP 200
[Asserts]
body == "OK HTTPS"

# Check the request: not HTTPS
GET {{rootUrl}}/ishttps

HTTP 200
[Asserts]
body == "OK FALLBACK"