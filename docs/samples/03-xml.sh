#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# SOAP XML
curl --location --request POST "$HTTPL_ROOT_URL/InStock" \
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
curl --location --request POST "$HTTPL_ROOT_URL/thingy" \
--header "Content-Type: application/soap+xml; charset=utf-8" \
--data-raw '<?xml version="1.0"?><object><a>TEST</a><b>TEST2</b></object>' -D-