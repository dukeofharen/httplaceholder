# Create stub: text JSONPath check
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: post-with-text-jsonpath-checker
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-with-text-jsonpath-checker
  headers:
    Content-Type: application/json
  jsonPath:
    - "$.phoneNumbers[?(@.type=='iPhone')]"
response:
  text: OK WITH JSONPATH TEXT CHECKER
```

HTTP 200

# Create stub: text and object JSONPath check
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: post-with-text-and-object-jsonpath-checker
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-with-text-and-object-jsonpath-checker
  headers:
    Content-Type: application/json
  jsonPath:
    - '$.people[0].age'
    - query: '$.people[0].firstName'
      expectedValue: 'John'
response:
  text: OK WITH JSONPATH TEXT AND OBJECT CHECKER
```

HTTP 200

# Check the request: text JSONPath check
POST {{rootUrl}}/post-with-text-jsonpath-checker
Content-Type: application/json
{
    "phoneNumbers": [
        {
            "type": "iPhone",
            "number": "0123-4567-8888"
        },
        {
            "type": "home",
            "number": "0123-4567-8910"
        }
    ]
}

HTTP 200
[Asserts]
body == "OK WITH JSONPATH TEXT CHECKER"

# Check the request: text JSONPath check and JSON invalid
POST {{rootUrl}}/post-with-text-jsonpath-checker
Content-Type: application/json
{
    "phoneNumbers": [
        {
            "type": "Android",
            "number": "0123-4567-8888"
        },
        {
            "type": "home",
            "number": "0123-4567-8910"
        }
    ]
}

HTTP 200
[Asserts]
body == "OK FALLBACK"

# Check the request: text and object JSONPath check
POST {{rootUrl}}/post-with-text-and-object-jsonpath-checker
Content-Type: application/json
{
  "people": [
    {
      "firstName": "John",
      "age": 29,
      "achievements": [
        {
          "name": "Just an average guy"
        }
      ]
    }
  ]
}

HTTP 200
[Asserts]
body == "OK WITH JSONPATH TEXT AND OBJECT CHECKER"

# Check the request: text and object JSONPath check and text checker invalid
POST {{rootUrl}}/post-with-text-and-object-jsonpath-checker
Content-Type: application/json
{
  "people": [
    {
      "firstName": "John",
      "achievements": [
        {
          "name": "Just an average guy"
        }
      ]
    }
  ]
}

HTTP 200
[Asserts]
body == "OK FALLBACK"

# Check the request: text and object JSONPath check and object checker invalid
POST {{rootUrl}}/post-with-text-and-object-jsonpath-checker
Content-Type: application/json
{
  "people": [
    {
      "firstName": "Marc",
      "age": 29,
      "achievements": [
        {
          "name": "Just an average guy"
        }
      ]
    }
  ]
}

HTTP 200
[Asserts]
body == "OK FALLBACK"