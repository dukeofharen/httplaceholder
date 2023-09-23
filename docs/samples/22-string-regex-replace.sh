#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# String replace
curl --location --request GET "$HTTPL_ROOT_URL/string-replace" -D-

# Regex replace
curl --location --request GET "$HTTPL_ROOT_URL/regex-replace?queryString=Bassie' -D-

# String and regex replace
curl --location --request GET '$HTTPL_ROOT_URL/string-and-regex-replace' -D-