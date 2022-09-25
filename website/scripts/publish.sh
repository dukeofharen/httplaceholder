#!/bin/bash
set -e
set -u

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
bash $DIR/build.sh

echo "Provide SSH port:"
read PORT
scp -P $PORT -r $DIR/../dist/* httplaceholder@httplaceholder.org:/var/www/httplaceholder.org