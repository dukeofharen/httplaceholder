# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-jsonpath
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /dynamic-mode-jsonpath
response:
  enableDynamicMode: true
  headers:
    X-Value: ((jsonpath:$.values[1].title))
  text: ((jsonpath:$.values[1].title))
```

HTTP 200

# Check the request
POST {{rootUrl}}/dynamic-mode-jsonpath
{
    "values": [
        {
            "title": "Value1"
        },
        {
            "title": "Value2"
        }
    ]
}

HTTP 200
[Asserts]
body == "Value2"
header "X-Value" == "Value2"