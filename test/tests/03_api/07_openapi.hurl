# Delete all stubs
DELETE {{rootUrl}}/ph-api/stubs
HTTP 204

# Import OpenAPI: pet store
POST {{rootUrl}}/ph-api/import/openapi
Content-Type: text/plain
file,07_openapi_petstore.yaml;
HTTP 200

[Asserts]
jsonpath "$" count == 6
jsonpath "$[0].stub.id" contains "generated-"
jsonpath "$[0].stub.conditions.method" == "GET"
jsonpath "$[0].stub.conditions.url.query.limit.equals" != ""
jsonpath "$[0].stub.conditions.headers['X-Api-Key'].equals" != ""
jsonpath "$[0].stub.conditions.host.equals" == "petstore.swagger.io"
jsonpath "$[0].stub.response.statusCode" == 200
jsonpath "$[0].stub.response.contentType" == "application/json"
jsonpath "$[0].stub.response.headers['x-next']" != ""

# Import OpenAPI: HttPlaceholder
POST {{rootUrl}}/ph-api/import/openapi
Content-Type: text/plain
file,07_openapi_httplaceholder.json;
HTTP 200

[Asserts]
jsonpath "$" count == 73
jsonpath "$[0].stub.id" contains "generated-"