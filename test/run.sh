#!/bin/bash
set -eu

HTTPL_ROOT_URL="http://localhost:5000"
VERBOSE=""

while getopts ":v:u:" o; do
  case "${o}" in
  v)
    VERBOSE=${OPTARG}
    ;;
  u)
    HTTPL_ROOT_URL=${OPTARG}
    ;;
  esac
done
shift $((OPTIND - 1))

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
echo "Running integration tests for URL $HTTPL_ROOT_URL"

COMMAND="hurl --test --glob '$DIR/tests/**/*.hurl' --variable rootUrl=$HTTPL_ROOT_URL"
if [ "$VERBOSE" = "verbose" ]; then
  COMMAND="$COMMAND --verbose"
fi

eval "$COMMAND"