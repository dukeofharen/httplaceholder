# No distribution key

## Delete all stubs
DELETE {{rootUrl}}/ph-api/stubs
HTTP 204

## Make request
GET {{rootUrl}}/request1

HTTP 501

[Captures]
corr_id1: header "X-HttPlaceholder-Correlation"

## Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: request-1
priority: -1
tenant: dist-key
conditions:
  url:
    path:
      equals: /request1
response:
  text: OK NO DISTRIBUTION KEY
```

HTTP 200

## Make request
GET {{rootUrl}}/request1

HTTP 200

[Captures]
corr_id2: header "X-HttPlaceholder-Correlation"

[Asserts]
body == "OK NO DISTRIBUTION KEY"

## Check requests
GET {{rootUrl}}/ph-api/requests/{{corr_id1}}

HTTP 200

[Asserts]
jsonpath "$.requestParameters.url" contains "/request1"

GET {{rootUrl}}/ph-api/requests/{{corr_id2}}

HTTP 200

[Asserts]
jsonpath "$.requestParameters.url" contains "/request1"
jsonpath "$.executingStubId" == "request-1"

# Check response
GET {{rootUrl}}/ph-api/requests/{{corr_id2}}/response

HTTP 200

[Asserts]
jsonpath "$.statusCode" == 200
jsonpath "$.body" == "T0sgTk8gRElTVFJJQlVUSU9OIEtFWQ=="

# With distribution key

## Delete all stubs
DELETE {{rootUrl}}/ph-api/stubs
X-HttPlaceholder-DistKey: dist1
HTTP 204

## Check response: not found
GET {{rootUrl}}/ph-api/requests/{{corr_id2}}/response
X-HttPlaceholder-DistKey: dist1

HTTP 404

## Make request
GET {{rootUrl}}/request1
X-HttPlaceholder-DistKey: dist1

HTTP 501

[Captures]
corr_id1: header "X-HttPlaceholder-Correlation"

## Create stub
POST {{rootUrl}}/ph-api/stubs
X-HttPlaceholder-DistKey: dist1
Content-Type: text/yaml
```
id: request-1
priority: -1
tenant: dist-key
conditions:
  url:
    path:
      equals: /request1
response:
  text: OK WITH DISTRIBUTION KEY
```

HTTP 200

## Make request
GET {{rootUrl}}/request1
X-HttPlaceholder-DistKey: dist1

HTTP 200

[Captures]
corr_id2: header "X-HttPlaceholder-Correlation"

[Asserts]
body == "OK WITH DISTRIBUTION KEY"

## Check response
GET {{rootUrl}}/ph-api/requests/{{corr_id2}}/response
X-HttPlaceholder-DistKey: dist1

HTTP 200

[Asserts]
jsonpath "$.statusCode" == 200
jsonpath "$.body" == "T0sgV0lUSCBESVNUUklCVVRJT04gS0VZ"

## Check requests
GET {{rootUrl}}/ph-api/requests/{{corr_id1}}
X-HttPlaceholder-DistKey: dist1

HTTP 200

[Asserts]
jsonpath "$.requestParameters.url" contains "/request1"

GET {{rootUrl}}/ph-api/requests/{{corr_id2}}
X-HttPlaceholder-DistKey: dist1

HTTP 200

[Asserts]
jsonpath "$.requestParameters.url" contains "/request1"
jsonpath "$.executingStubId" == "request-1"