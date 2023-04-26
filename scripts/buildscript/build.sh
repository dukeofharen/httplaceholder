#!/bin/bash
set -e
set -u

# TODO build UI in one big JS file
# TODO NuGet and NPM version upgrades

ROOT_PATH="$1"
if [ "$ROOT_PATH" = "" ]; then
  echo "ROOT_PATH variable not set"
  exit 1  
fi

BUILDSCRIPTS_FOLDER="$ROOT_PATH/scripts/buildscript"
BUILD_METADATA_PATH="$ROOT_PATH/.build"
DIST_PATH="$ROOT_PATH/dist"
chmod -R +x "$BUILDSCRIPTS_FOLDER"

echo "Building HttPlaceholder"

# Ensure .build folder
if [ ! -d "$BUILD_METADATA_PATH" ]; then
  echo "Creating directory $BUILD_METADATA_PATH"
  mkdir "$BUILD_METADATA_PATH"  
fi

# Ensure dist folder
if [ -d "$DIST_PATH" ]; then
  echo "Deleting directory $DIST_PATH"
  rm -rf "$DIST_PATH"  
fi

echo "Creating directory $DIST_PATH"
mkdir "$DIST_PATH"

# Ensure software
bash "$BUILDSCRIPTS_FOLDER/ensure-software.sh"

# Get and set version
bash "$BUILDSCRIPTS_FOLDER/set-version.sh" "$ROOT_PATH"
VERSION=$(cat $ROOT_PATH/version.txt)

# Run unit tests of .NET project
bash "$BUILDSCRIPTS_FOLDER/run-tests.sh" "$ROOT_PATH"

# Build docs
bash "$BUILDSCRIPTS_FOLDER/build-docs.sh" "$ROOT_PATH"

# Build UI
bash "$BUILDSCRIPTS_FOLDER/build-ui.sh" "$ROOT_PATH"

# Build all packages
bash "$BUILDSCRIPTS_FOLDER/build-linux.sh" "$VERSION" "$ROOT_PATH"
bash "$BUILDSCRIPTS_FOLDER/build-nuget-client.sh" "$VERSION" "$ROOT_PATH"
bash "$BUILDSCRIPTS_FOLDER/build-osx.sh" "$VERSION" "$ROOT_PATH"
bash "$BUILDSCRIPTS_FOLDER/build-tool.sh" "$VERSION" "$ROOT_PATH"
bash "$BUILDSCRIPTS_FOLDER/build-windows.sh" "$VERSION" "$ROOT_PATH"
bash "$BUILDSCRIPTS_FOLDER/create-open-api-file.sh" "$ROOT_PATH"

# TODO execute Postman tests