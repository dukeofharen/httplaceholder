# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-display-url
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-display-url
response:
  enableDynamicMode: true
  headers:
    X-DisplayUrl: 'Header display url: ((display_url))'
  text: 'Display url: ((display_url))'
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-display-url-regex-example
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-display-url-regex/users/123/orders
response:
  enableDynamicMode: true
  text: "User ID: ((display_url:'\/users\/([0-9]{3})\/orders'))"
  headers:
    X-Header: "((display_url:'\/users\/([0-9]{3})\/orders'))"
priority: 0
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-display-url?q1=var1&q2=var2

HTTP 200
[Asserts]
body == "Display url: {{rootUrl}}/dynamic-mode-display-url?q1=var1&q2=var2"
header "X-DisplayUrl" == "Header display url: {{rootUrl}}/dynamic-mode-display-url?q1=var1&q2=var2"

# Check the request: regex
GET {{rootUrl}}/dynamic-display-url-regex/users/123/orders?key=val

HTTP 200
[Asserts]
body == "User ID: 123"
header "X-Header" == "123"