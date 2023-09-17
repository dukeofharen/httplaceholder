# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-root-url
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-root-url
response:
  enableDynamicMode: true
  headers:
    X-RootUrl: 'Header root url: ((root_url))'
  text: 'Root url: ((root_url))'
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-root-url?q1=var1&q2=var2

HTTP 200
[Asserts]
body == "Root url: {{rootUrl}}"
header "X-RootUrl" == "Header root url: {{rootUrl}}"