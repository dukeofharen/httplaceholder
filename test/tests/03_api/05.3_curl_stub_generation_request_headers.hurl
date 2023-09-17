# Import cURL
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  -H 'X-Header1: value 1' \
  -H 'X-Header2: value 2' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.headers['X-Header1'].equals" == "value 1"
jsonpath "$[0].stub.conditions.headers['X-Header2'].equals" == "value 2"
jsonpath "$[0].stub.conditions.headers['Accept-Encoding'].equals" == "deflate, gzip, br"