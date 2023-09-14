#!/bin/bash
set -e
set -u

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
HTTPL_ROOT_URL="${1-http://localhost:5000}"
echo "Running integration tests for URL $HTTPL_ROOT_URL"

hurl --test --glob "$DIR/tests/**/*.hurl" --variable rootUrl=$HTTPL_ROOT_URL --verbose