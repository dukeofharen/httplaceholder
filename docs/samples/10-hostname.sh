#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Regex
curl --location --request GET "$HTTPL_ROOT_URL/host-2" --header "Host: httplaceholder2.local" -D-

# Full
curl --location --request GET "$HTTPL_ROOT_URL/host-1" --header "Host: httplaceholder.local" -D-