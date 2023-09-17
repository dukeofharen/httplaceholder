# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-client-ip
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-client-ip
response:
  enableDynamicMode: true
  headers:
    X-ClientIp: 'Header client ip: ((client_ip))'
  text: 'Client ip: ((client_ip))'
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-client-ip
X-Forwarded-For: 11.22.33.44

HTTP 200
[Asserts]
body == "Client ip: 11.22.33.44"
header "X-ClientIp" == "Header client ip: 11.22.33.44"