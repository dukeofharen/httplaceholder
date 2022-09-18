#!/bin/bash
set -e
set -u

npm run build-site-prod

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
scp -r $DIR/../dist/* budgetkar@budgetkar.nl:/var/www/ducode.org