#!/bin/bash
set -e
set -u

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
bash $DIR/build.sh
scp -r $DIR/../dist/* budgetkar@budgetkar.nl:/var/www/httplaceholder.org