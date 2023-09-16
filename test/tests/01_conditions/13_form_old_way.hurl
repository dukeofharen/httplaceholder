# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: form-old-way
conditions:
  method: POST
  url:
    path: /form-old-way
  form:
    - key: key1
      value: val1
    - key: key2
      value: val2
response:
  text: form-old-way-ok
```

HTTP 200

# Check the request
POST {{rootUrl}}/form-old-way
[FormParams]
key1: val1
key2: val2

HTTP 200
[Asserts]
body == "form-old-way-ok"

# Check the request: invalid input
POST {{rootUrl}}/form-old-way
[FormParams]
key1: val1
key2: val3

HTTP 200
[Asserts]
body == "OK FALLBACK"