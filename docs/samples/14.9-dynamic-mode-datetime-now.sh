#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Local
curl --location --request GET "$HTTPL_ROOT_URL/dynamic-local-now.txt' -D-

# UTC
curl --location --request GET '$HTTPL_ROOT_URL/dynamic-utc-now.txt' -D-