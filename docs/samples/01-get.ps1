$response = Invoke-WebRequest "http://localhost:5000/users?id=12&filter=first_name" -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="

$response = Invoke-WebRequest "http://localhost:5000/users?id=14&filter=last_name" -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="

$response = Invoke-WebRequest "http://localhost:5000/users?id=15&filter=last_name&last_name=Johnson" -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="

$response = Invoke-WebRequest "http://localhost:5000/users?id=18" -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="

$response = Invoke-WebRequest "http://localhost:5000/users?filter=first_name" -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="