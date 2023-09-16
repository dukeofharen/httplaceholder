# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: reverse-proxy
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /reverse-proxy
response:
  reverseProxy:
    url: {{rootUrl}}/proxy-destination
    appendPath: true
    appendQueryString: true
    replaceRootUrl: true
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: reverse-proxy-destination
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /proxy-destination
response:
  text: REVERSE PROXY DESTINATION
  headers:
    X-Proxy-1: REVERSE PROXY HEADER
```

HTTP 200

# Check the request
GET {{rootUrl}}/reverse-proxy

HTTP 200
[Asserts]
body == "REVERSE PROXY DESTINATION"
header "X-Proxy-1" == "REVERSE PROXY HEADER"