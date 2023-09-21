#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
curl --location --request GET "$HTTPL_ROOT_URL/dynamic-request-header.txt' \
--header 'X-Api-Key: abc123' -D-