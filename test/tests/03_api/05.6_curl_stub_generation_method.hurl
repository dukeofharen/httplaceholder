# Import cURL: implicit POST
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  --data-raw '{}' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.method" == "POST"

# Import cURL: implicit GET
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.method" == "GET"

# Import cURL: explicit PUT
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  -X 'PUT' \
  --data-raw '{}' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.method" == "PUT"