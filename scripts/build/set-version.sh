#!/bin/bash
if [ "$1" = "" ]; then
    echo "Build number not set"
    exit 1
fi

set -e
set -u

BUILD_NUMBER="$1"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_FOLDER="$DIR/../.."

echo "Determining version"
YEAR="$(date +%Y)"

MONTH=$(date +%m)
MONTH="${MONTH#0}"

DAY="$(date +%d)"
DAY="${DAY#0}"

VERSION="$YEAR.$MONTH.$DAY.$BUILD_NUMBER"
echo "Version is $VERSION"
echo "$VERSION" > "$ROOT_FOLDER/version.txt"