#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Regular
curl --location --request GET "$HTTPL_ROOT_URL/slooooow?id=12&filter=first_name" -D-

# Regular min/max
curl --location --request GET "$HTTPL_ROOT_URL/slooooow-min-max?id=12&filter=first_name" -D-