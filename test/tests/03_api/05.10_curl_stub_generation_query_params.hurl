# Import cURL: JSON object
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path?param1=val1&param2=val2' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.url.query.param1.equals" == "val1"
jsonpath "$[0].stub.conditions.url.query.param2.equals" == "val2"