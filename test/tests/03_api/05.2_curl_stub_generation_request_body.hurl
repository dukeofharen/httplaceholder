# Import cURL
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  -H 'Content-Type: text/plain' \
  --data-raw 'Some request body' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.body[0].equals" == "Some request body"