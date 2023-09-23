#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
curl --location --request GET "$HTTPL_ROOT_URL/auth?id=12&filter=first_name" \
--header "Authorization: Basic dXNlcjpwYXNz" -D-