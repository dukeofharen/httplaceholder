# Import cURL: JSON object
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.url.path.equals" == "/request-path"