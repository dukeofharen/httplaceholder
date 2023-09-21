#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
curl --location --request POST "$HTTPL_ROOT_URL/dynamic-mode-jsonpath.txt" \
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