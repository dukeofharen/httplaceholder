#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Regular
curl --location --request GET "$HTTPL_ROOT_URL/dynamic-query.txt?response_text=This_is_the_response_text&response_header=This_is_the_response_header" -D-

# Encoded
curl --location --request GET "$HTTPL_ROOT_URL/dynamic-query.txt?response_text=This_is_the_response_text&response_header=This_is_the_response_header" -D-