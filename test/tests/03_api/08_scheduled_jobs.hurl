# Run clean old requests job
POST {{rootUrl}}/ph-api/scheduledJob/CleanOldRequestsJob
HTTP 200

[Asserts]
jsonpath "$.message" == "OK"

# Get scheduled job names
GET {{rootUrl}}/ph-api/scheduledJob
HTTP 200

[Asserts]
jsonpath "$" count == 1
jsonpath "$[0]" == "CleanOldRequestsJob"