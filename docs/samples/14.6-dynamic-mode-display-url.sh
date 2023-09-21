#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Regular
curl --location --request GET "$HTTPL_ROOT_URL/dynamic-display-url.txt?testvar=value1" -D-

# Regex
curl --location --request GET '$HTTPL_ROOT_URL/dynamic-display-url-regex/users/451/orders' -D-