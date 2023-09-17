# Import cURL: multipart
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  -F 'param1=value1&param2=value2' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.headers['Content-Type'].equals" == "multipart/form-data"
jsonpath "$[0].stub.conditions.form[0].key" == "param1"
jsonpath "$[0].stub.conditions.form[0].value.equals" == "value1"
jsonpath "$[0].stub.conditions.form[1].key" == "param2"
jsonpath "$[0].stub.conditions.form[1].value.equals" == "value2"

# Import cURL: form encoded
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/request-path' \
  -d 'param1=value1&param2=value2' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.headers['Content-Type'].equals" == "application/x-www-form-urlencoded"
jsonpath "$[0].stub.conditions.form[0].key" == "param1"
jsonpath "$[0].stub.conditions.form[0].value.equals" == "value1"
jsonpath "$[0].stub.conditions.form[1].key" == "param2"
jsonpath "$[0].stub.conditions.form[1].value.equals" == "value2"