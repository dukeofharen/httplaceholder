#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Enforce Unix line endings
curl --location --request GET "$HTTPL_ROOT_URL/unix-line-endings" -D-

# Enforce Windows line endings
curl --location --request GET "$HTTPL_ROOT_URL/windows-line-endings" -D-