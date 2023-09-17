# Delete all stubs
DELETE {{rootUrl}}/ph-api/stubs

# Import HAR
POST {{rootUrl}}/ph-api/import/har?tenant=tenant1
Content-Type: text/plain
file,06_har.har;

HTTP 200

[Asserts]
jsonpath "$" count == 1

# Check stub conditions
jsonpath "$[0].stub.id" == "generated-cc584a2391a95c12edd0933256e5a958"
jsonpath "$[0].stub.conditions.method" == "GET"
jsonpath "$[0].stub.conditions.url.path.equals" == "/"
jsonpath "$[0].stub.conditions.url.isHttps" == true
jsonpath "$[0].stub.conditions.headers['cache-control'].equals" == "no-cache"
jsonpath "$[0].stub.conditions.host.equals" == "ducode.org"

# Check stub response
jsonpath "$[0].stub.response.statusCode" == 200
jsonpath "$[0].stub.response.contentType" == "text/html"
jsonpath "$[0].stub.response.headers['server']" == "nginx"
jsonpath "$[0].stub.response.html" contains "<!DOCTYPE html>"