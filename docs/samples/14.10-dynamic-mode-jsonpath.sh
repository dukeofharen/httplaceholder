#!/bin/bash
curl --location --request POST "http://localhost:5000/dynamic-mode-jsonpath.txt" \
--header "Content-Type: application/json" \
--data-raw '{
    "values": [
        {
            "title": "Value1"
        },
        {
            "title": "Value2"
        }
    ]
}' -D-