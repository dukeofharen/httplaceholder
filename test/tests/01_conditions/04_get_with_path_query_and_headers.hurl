# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: get-path-query-and-headers
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /get
    query:
      q1:
        equals: val1
      q2:
        equals: val2
  headers:
    X-Header-1:
      equals: value1
    X-Header-2:
      equals: value2
    X-Header-3:
      present: true
    X-Header-4:
      present: false
response:
  text: OK PATH, QUERY AND HEADERS
```

HTTP 200

# Check the request
GET {{rootUrl}}/get?q1=val1&q2=val2
X-Header-1: value1
X-Header-2: value2
X-Header-3: somevalue

HTTP 200
[Asserts]
body equals "OK PATH, QUERY AND HEADERS"

# Check the request: path is incorrect
GET {{rootUrl}}/post?q1=val1&q2=val2
X-Header-1: value1
X-Header-2: value2
X-Header-3: somevalue

HTTP 200
[Asserts]
body equals "OK FALLBACK"

# Check the request: query is incorrect
GET {{rootUrl}}/get?q1=val1&q2=val3
X-Header-1: value1
X-Header-2: value2
X-Header-3: somevalue

HTTP 200
[Asserts]
body equals "OK FALLBACK"

# Check the request: header is incorrect
GET {{rootUrl}}/get?q1=val1&q2=val2
X-Header-1: value3
X-Header-2: value2
X-Header-3: somevalue
X-Header-4: another value, should not be here
X-Header-5: some other unused value

HTTP 200
[Asserts]
body equals "OK FALLBACK"

# Check the request: header presence check fails
GET {{rootUrl}}/get?q1=val1&q2=val2
X-Header-1: value1
X-Header-2: value2

HTTP 200
[Asserts]
body equals "OK FALLBACK"