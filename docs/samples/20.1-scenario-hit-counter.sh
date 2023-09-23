#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Min hits
curl --location --request GET "$HTTPL_ROOT_URL/min-hits" -D-

# Max hits
curl --location --request GET "$HTTPL_ROOT_URL/max-hits" -D-

# Exact hits
curl --location --request GET "$HTTPL_ROOT_URL/exact-hits' -D-