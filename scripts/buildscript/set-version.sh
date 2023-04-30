#!/bin/bash
set -e
set -u

ROOT_FOLDER="$1"
if [ "$ROOT_FOLDER" = "" ]; then
  echo "ROOT_FOLDER variable not set."
  exit 1
fi

BUILD_METADATA_PATH="$ROOT_FOLDER/.build"

echo "Determining build number"
BUILD_NUMBER_PATH="$BUILD_METADATA_PATH/buildnumber"
if [ ! -f "$BUILD_NUMBER_PATH" ]; then
  echo "Creating file $BUILD_NUMBER_PATH"
  touch $BUILD_NUMBER_PATH
fi

BUILD_NUMBER=$(cat $BUILD_NUMBER_PATH)
if [ "$BUILD_NUMBER" = "" ]; then
  echo "1" > "$BUILD_NUMBER_PATH"
  BUILD_NUMBER="1"
else
  BUILD_NUMBER=$(($BUILD_NUMBER+1))
  echo $BUILD_NUMBER > "$BUILD_NUMBER_PATH"
fi

CHANGELOG_PATH="$ROOT_FOLDER/CHANGELOG"

echo "Determining version"
BASE_VERSION=$(head -n 1 $CHANGELOG_PATH | sed 's/\[//' | sed 's/\]//')
if [ "$BASE_VERSION" = "vnext" ]; then
  BASE_VERSION=$(date +"%Y.%-m.%-d")
fi

VERSION="$BASE_VERSION.$BUILD_NUMBER"
echo "Version is $VERSION"
echo "$VERSION" > "$ROOT_FOLDER/version.txt"