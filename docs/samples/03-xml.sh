#!/bin/bash
# SOAP XML
curl --location --request POST "http://localhost:5000/InStock" \
--header "Content-Type: application/soap+xml; charset=utf-8" \
--data-raw '<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" 
    xmlns:m="http://www.example.org/stock/Reddy">
    <soap:Header></soap:Header>
    <soap:Body>
        <m:GetStockPrice>
            <m:StockName>GOOG</m:StockName>
        </m:GetStockPrice>
    </soap:Body>
</soap:Envelope>' -D-

# Regular XML
curl --location --request POST "http://localhost:5000/thingy" \
--header "Content-Type: application/soap+xml; charset=utf-8" \
--data-raw '<?xml version="1.0"?><object><a>TEST</a><b>TEST2</b></object>' -D-