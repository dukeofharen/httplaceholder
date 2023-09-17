# Import cURL: JSON object
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/json-object' \
  -H 'Content-Type: application/json' \
  --data-raw '{"stringValue":"value","intValue":42.1,"boolValue":true,"objectValue":{"stringValue":"anotherValue"},"arrayValue":["value1",333]}' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.json.stringValue" == "value"
jsonpath "$[0].stub.conditions.json.intValue" == 42.1
jsonpath "$[0].stub.conditions.json.boolValue" == true
jsonpath "$[0].stub.conditions.json.objectValue.stringValue" == "anotherValue"
jsonpath "$[0].stub.conditions.json.arrayValue" count == 2
jsonpath "$[0].stub.conditions.json.arrayValue[0]" == "value1"
jsonpath "$[0].stub.conditions.json.arrayValue[1]" == 333

# Import cURL: JSON array
POST {{rootUrl}}/ph-api/import/curl
Content-Type: text/plain
```
curl 'https://api.site.com/json-object' \
  -H 'Content-Type: application/json' \
  --data-raw '["value1",44,false,{"key1":"val1"},["1",2]]' \
  --compressed
```

HTTP 200

[Asserts]
jsonpath "$[0].stub.conditions.json" count == 5
jsonpath "$[0].stub.conditions.json[0]" == "value1"
jsonpath "$[0].stub.conditions.json[1]" == 44
jsonpath "$[0].stub.conditions.json[2]" == false
jsonpath "$[0].stub.conditions.json[3].key1" == "val1"
jsonpath "$[0].stub.conditions.json[4]" count == 2
jsonpath "$[0].stub.conditions.json[4][0]" == "1"
jsonpath "$[0].stub.conditions.json[4][1]" == 2