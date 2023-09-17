# Retrieve config
GET {{rootUrl}}/ph-api/configuration
HTTP 200

[Asserts]
jsonpath "$" count > 0

# Set storeResponses to "false"
PATCH {{rootUrl}}/ph-api/configuration
{
    "configurationKey": "storeResponses",
    "newValue": "false"
}

HTTP 204

# Check config
GET {{rootUrl}}/ph-api/configuration
HTTP 200

[Asserts]
jsonpath "$[?(@.key=='storeResponses')].value" includes "false"

# Set storeResponses to "true"
PATCH {{rootUrl}}/ph-api/configuration
{
    "configurationKey": "storeResponses",
    "newValue": "true"
}

HTTP 204

# Check config
GET {{rootUrl}}/ph-api/configuration
HTTP 200

[Asserts]
jsonpath "$[?(@.key=='storeResponses')].value" includes "true"