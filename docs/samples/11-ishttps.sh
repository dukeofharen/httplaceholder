#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
curl --location --request GET "$HTTPL_HTTPS_ROOT_URL/ishttps-ok" -D- -k