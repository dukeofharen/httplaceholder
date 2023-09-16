# Import cURL: authorization header
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/api/v1/users/authenticate' \
  -H 'authorization: Basic dXNlcjpwYXNz' \
  --data-raw '{}' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.basicAuthentication.username" == "user"
jsonpath "$[0].stub.conditions.basicAuthentication.password" == "pass"