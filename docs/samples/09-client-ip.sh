#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Exact IP
curl --location --request GET "$HTTPL_ROOT_URL/client-ip-1" -D-

# IP range
curl --location --request GET "$HTTPL_ROOT_URL/client-ip-2" -D-