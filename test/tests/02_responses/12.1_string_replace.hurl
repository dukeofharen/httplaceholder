# Create stubs
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: string-replace
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /string-replace
response:
  text: REPLACE THIS
  replace:
    - text: REPLACE 
      ignoreCase: false
      replaceWith: OK
    - text: THIS
      ignoreCase: false
      replaceWith: STRING REPLACE
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: string-replace-ignore-case
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /string-replace-ignore-case
response:
  text: REPLACE THIS
  replace:
    - text: replace 
      ignoreCase: true
      replaceWith: OK
    - text: this
      ignoreCase: true
      replaceWith: STRING REPLACE
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: string-replace-dynamic
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /string-replace-dynamic
response:
  enableDynamicMode: true
  text: REPLACE THIS
  replace:
    - text: REPLACE 
      replaceWith: ((query:q1))
    - text: THIS
      replaceWith: ((query:q2))
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: string-replace-proxy-base
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /string-replace-proxy-base
response:
  text: HELLO FROM STRING REPLACE PROXY
```

HTTP 200

POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: string-replace-proxy
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /string-replace-proxy
response:
  text: REPLACE THIS
  reverseProxy:
    url: {{rootUrl}}/string-replace-proxy-base
    appendPath: true
    appendQueryString: true
    replaceRootUrl: true
  replace:
    - text: HELLO 
      replaceWith: BYE
```

HTTP 200

# Check the request: string replace
GET {{rootUrl}}/string-replace

HTTP 200
[Asserts]
body == "OK STRING REPLACE"

# Check the request: string replace ignore casing
GET {{rootUrl}}/string-replace-ignore-case

HTTP 200
[Asserts]
body == "OK STRING REPLACE"

# Check the request: string replace with dynamic mode
GET {{rootUrl}}/string-replace-dynamic?q1=val1&q2=val2

HTTP 200
[Asserts]
body == "val1 val2"

# Check the request: string replace with proxy
GET {{rootUrl}}/string-replace-proxy

HTTP 200
[Asserts]
body == "BYE FROM STRING REPLACE PROXY"