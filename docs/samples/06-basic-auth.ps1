$url = "http://localhost:5000/users?id=12&filter=first_name"
$headers = @{"Authorization" = "Basic dXNlcjpwYXNz"}
$response = Invoke-WebRequest $url -Method Get -Headers $headers -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="