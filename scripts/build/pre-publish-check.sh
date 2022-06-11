#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_FOLDER="$DIR/../.."
CHANGELOG_PATH="$ROOT_FOLDER/CHANGELOG"
BASE_VERSION=$(head -n 1 $CHANGELOG_PATH | sed 's/\[//' | sed 's/\]//')
if [ "$BASE_VERSION" = "vnext" ]; then
  echo "Version as set in $CHANGELOG_PATH is still 'vnext'. Please change it to a correct version"
  exit 1
fi

echo "Version $BASE_VERSION is correct; continuing..."