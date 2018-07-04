$response = Invoke-WebRequest "http://localhost:5000/users?id=12&filter=first_name" -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="