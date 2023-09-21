#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Regex
curl --location --request POST "$HTTPL_ROOT_URL/dynamic-request-body-regex.txt' \
--header 'Content-Type: text/plain' \
--data-raw 'key1=value1
key2=value2
key3=value3' -D-

# Regular
curl --location --request POST '$HTTPL_ROOT_URL/dynamic-request-body.txt' \
--header 'Content-Type: text/plain' \
--data-raw 'TEST VALUE' -D-