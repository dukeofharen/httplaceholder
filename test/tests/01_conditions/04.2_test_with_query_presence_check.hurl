# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-presence-check
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /get-presence-check
    query:
      q1:
        present: true
      q2:
        present: true
      q3:
        present: false
response:
  text: OK GET PRESENCE CHECK
```

HTTP 200

# Check the request
GET {{rootUrl}}/get-presence-check?q1=val1&q2=val3

HTTP 200
[Asserts]
body == "OK GET PRESENCE CHECK"

# Check the request: check fails
GET {{rootUrl}}/get-presence-check?q1=val1&q2=val2&q3=val3

HTTP 200
[Asserts]
body == "OK FALLBACK"