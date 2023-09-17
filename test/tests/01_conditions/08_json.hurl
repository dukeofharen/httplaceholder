# Create stub: JSON object checker
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: post-with-json-object-checker
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-with-json-object-checker
  json:
    strValue: stringInput
    boolValue: true
    doubleValue: 1.23
    dateTimeValue: 2021-04-18T13:14:10
    intValue: 4
    nullValue: null
    subObject:
      subObjKey1: subVal1
      subObjKey2: subVal2
    arrayValue:
      - val1
      - val2
      - subKey1: subVal1
        subKey2: subVal2
response:
  text: OK POST WITH JSON OBJECT CHECKER
```

HTTP 200

# Create stub: JSON array checker
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: post-with-json-array-checker
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-with-json-array-checker
  json:
    - stringInput
    - true
    - 1.23
    - 2021-04-18T13:14:10
    - 4
    - subObjKey1: subVal1
      subObjKey2: subVal2
    - - val1
      - val2
      - subKey1: subVal1
        subKey2: subVal2
response:
  text: OK POST WITH JSON ARRAY CHECKER
```

HTTP 200

# Check the request: JSON object check
POST {{rootUrl}}/post-with-json-object-checker
Content-Type: application/json
{
    "strValue": "stringInput",
    "boolValue": true,
    "doubleValue": 1.23,
    "dateTimeValue": "2021-04-18T13:14:10",
    "intValue": 4,
    "nullValue": null,
    "subObject": {
        "subObjKey1": "subVal1",
        "subObjKey2": "subVal2"
    },
    "arrayValue": [
        "val1",
        "val2",
        {
            "subKey1": "subVal1",
            "subKey2": "subVal2"
        }
    ]
}

HTTP 200
[Asserts]
body == "OK POST WITH JSON OBJECT CHECKER"

# Check the request: JSON object check and JSON incorrect
POST {{rootUrl}}/post-with-json-object-checker
Content-Type: application/json
{
    "strValue": "stringInput",
    "boolValue": true,
    "doubleValue": 1.23,
    "dateTimeValue": "2021-04-18T13:14:10",
    "intValue": 4,
    "nullValue": null,
    "subObject": {
        "subObjKey1": "subVal1",
        "subObjKey2": "subVal2"
    },
    "arrayValue": [
        "val1",
        "val2",
        {
            "subKey1": "subVal3",
            "subKey2": "subVal2"
        }
    ]
}

HTTP 200
[Asserts]
body == "OK FALLBACK"

# Check the request: JSON array check
POST {{rootUrl}}/post-with-json-array-checker
Content-Type: application/json
[
    "stringInput",
    true,
    1.23,
    "2021-04-18T13:14:10",
    4,
    {
        "subObjKey1": "subVal1",
        "subObjKey2": "subVal2"
    },
    [
        "val1",
        "val2",
        {
            "subKey1": "subVal1",
            "subKey2": "subVal2"
        }
    ]
]

HTTP 200
[Asserts]
body == "OK POST WITH JSON ARRAY CHECKER"

# Check the request: JSON array check and JSON invalid
POST {{rootUrl}}/post-with-json-array-checker
Content-Type: application/json
[
    "stringInput",
    true,
    1.23,
    "2021-04-18T13:14:11",
    4,
    {
        "subObjKey1": "subVal1",
        "subObjKey2": "subVal2"
    },
    [
        "val1",
        "val2",
        {
            "subKey1": "subVal1",
            "subKey2": "subVal2"
        }
    ]
]

HTTP 200
[Asserts]
body == "OK FALLBACK"