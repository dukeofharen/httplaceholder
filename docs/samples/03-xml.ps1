$url = "http://localhost:5000/InStock"
$headers = @{"Content-Type" = "application/soap+xml; charset=utf-8"}
$body = @"
<?xml version="1.0"?><soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:m="http://www.example.org/stock/Reddy"><soap:Header></soap:Header><soap:Body><m:GetStockPrice><m:StockName>GOOG</m:StockName>      </m:GetStockPrice></soap:Body></soap:Envelope>
"@
$response = Invoke-WebRequest $url -Method Post -Body $body -Headers $headers -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="

$url = "http://localhost:5000/thingy"
$headers = @{"Content-Type" = "application/soap+xml; charset=utf-8"}
$body = @"
<?xml version="1.0"?><object><a>TEST</a><b>TEST2</b></object>
"@
$response = Invoke-WebRequest $url -Method Post -Body $body -Headers $headers -UseBasicParsing
Write-Host $response.Content
Write-Host "=========="