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
CHANGELOG_PATH="$ROOT_FOLDER/CHANGELOG"

echo "Determining version"
BASE_VERSION=$(head -n 1 $CHANGELOG_PATH | sed 's/\[//' | sed 's/\]//')
VERSION="$BASE_VERSION.$1"
echo "Version is $VERSION"
echo "$VERSION" > "$ROOT_FOLDER/version.txt"