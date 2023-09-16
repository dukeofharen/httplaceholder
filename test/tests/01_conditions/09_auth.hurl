# Create stub: basic auth
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-with-basic-auth
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /basic-auth
  basicAuthentication:
    username: user1
    password: 'Secret1!'
response:
  text: OK BASIC AUTH
```

HTTP 200

# Check the request
GET {{rootUrl}}/basic-auth
[BasicAuth]
user1: Secret1!

HTTP 200
[Asserts]
body equals "OK BASIC AUTH"

# Check the request: incorrect credentials
GET {{rootUrl}}/basic-auth
[BasicAuth]
user1: Secret1@

HTTP 200
[Asserts]
body equals "OK FALLBACK"