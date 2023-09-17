# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: post-with-form
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-with-form
  form:
    - key: key1
      value:
        equals: val1
    - key: key2
      value:
        equals: val2-1
    - key: key2
      value:
        equals: val2-2
    - key: key3
      value:
        present: true
    - key: key4
      value:
        present: false
response:
  text: OK FORM
```

HTTP 200

# Check the request
POST {{rootUrl}}/post-with-form
[FormParams]
key1:val1
key2:val2-1
key2:val2-2
key3:any value

HTTP 200
[Asserts]
body == "OK FORM"

# Check the request: incorrect form data
POST {{rootUrl}}/post-with-form
[FormParams]
key1:val1
key2:val2-1
key2:val2-3

HTTP 200
[Asserts]
body == "OK FALLBACK"

# Check the request: presence check fails
POST {{rootUrl}}/post-with-form
[FormParams]
key1:val1
key2:val2-1
key2:val2-2
key3:any value
key4:should NOT be filled

HTTP 200
[Asserts]
body == "OK FALLBACK"