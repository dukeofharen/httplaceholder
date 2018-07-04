$url = "http://localhost:5000/users"
$headers = @{"X-Api-Key" = "123abc"; "X-Another-Secret" = "72354deg"}
$body = @"
{"username": "john"}
"@
$response = Invoke-WebRequest $url -Method Post -Body $body -Headers $headers -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="