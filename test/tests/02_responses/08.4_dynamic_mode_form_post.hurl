# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-form-post
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /dynamic-mode-form-post
response:
  enableDynamicMode: true
  headers:
    X-FormPost: 'Header form post: ((form_post:var1))'
  text: 'Form post: ((form_post:var2))'
```

HTTP 200

# Check the request
POST {{rootUrl}}/dynamic-mode-form-post
[FormParams]
var1: val1
var2: val2


HTTP 200
[Asserts]
body == "Form post: val2"
header "X-FormPost" == "Header form post: val1"