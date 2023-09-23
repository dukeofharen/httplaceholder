#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
curl --location --request POST "$HTTPL_ROOT_URL/users" \
--header "Content-Type: application/x-www-form-urlencoded" \
--header "X-Api-Key: 123abc" \
--header "X-Another-Secret: 72354deg" \
--data-urlencode "username=john" -D-