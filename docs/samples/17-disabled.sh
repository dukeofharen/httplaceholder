#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Enabled
curl --location --request GET "$HTTPL_ROOT_URL/enabled" -D-

# Disabled
curl --location --request GET "$HTTPL_ROOT_URL/disabled" -D-