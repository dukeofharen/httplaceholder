# Create stub: single IP check
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-with-client-ip-check
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /single-client-ip
  clientIp: 11.22.33.44
response:
  text: OK CLIENT IP CHECK
```

HTTP 200

# Create stub: IP range check
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-with-client-ip-range
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /client-ip-range
  clientIp: 11.22.33.00/30
response:
  text: OK CLIENT IP CHECK RANGE
```

HTTP 200

# Check the request: single IP check
GET {{rootUrl}}/single-client-ip
X-Forwarded-For: 11.22.33.44

HTTP 200
[Asserts]
body == "OK CLIENT IP CHECK"

# Check the request: single IP check and IP incorrect
GET {{rootUrl}}/single-client-ip
X-Forwarded-For: 11.22.33.55

HTTP 200
[Asserts]
body == "OK FALLBACK"

# Check the request: IP range check
GET {{rootUrl}}/client-ip-range
X-Forwarded-For: 11.22.33.3

HTTP 200
[Asserts]
body == "OK CLIENT IP CHECK RANGE"

# Check the request: IP range check and IP incorrect
GET {{rootUrl}}/client-ip-range
X-Forwarded-For: 11.22.33.4

HTTP 200
[Asserts]
body == "OK FALLBACK"