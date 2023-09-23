#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh

curl --location --request POST "$HTTPL_ROOT_URL/form" \
--header "Content-Type: application/x-www-form-urlencoded" \
--data-urlencode "key1=sjaak" \
--data-urlencode "key2=bob" \
--data-urlencode "key2=ducoo" -D-