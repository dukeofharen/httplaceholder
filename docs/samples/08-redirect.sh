#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Temporary
curl --location --request GET "$HTTPL_ROOT_URL/temp-redirect"

# Permanent
curl --location --request GET "$HTTPL_ROOT_URL/permanent-redirect" -D-