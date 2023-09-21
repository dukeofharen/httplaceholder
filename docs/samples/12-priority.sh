#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Fallback scenario
curl --location --request GET "$HTTPL_ROOT_URL/adsfasdfgas' -D-

# "Not" fallback scenario
curl --location --request GET '$HTTPL_ROOT_URL/not-fallback' -D-