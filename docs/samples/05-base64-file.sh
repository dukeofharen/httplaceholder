#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Read file from disk
curl --location --request GET "$HTTPL_ROOT_URL/cat_file.jpg" -D-

# Read file from stub
curl --location --request GET "$HTTPL_ROOT_URL/cat.jpg" -D-