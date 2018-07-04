$url = "http://localhost:5000/users"
$headers = @{"Content-Type" = "application/json"}
$body = @"
{"firstName":"John","lastName":"doe","age":26,"address":{"streetAddress":"naist street","city":"Nara","postalCode":"630-0192"},"phoneNumbers":[{"type":"iPhone","number":"0123-4567-8888"},{"type":"home","number":"0123-4567-8910"}]}
"@
$response = Invoke-WebRequest $url -Method Put -Body $body -Headers $headers -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="