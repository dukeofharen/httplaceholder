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

# Setup publish variables
bash "$BUILDSCRIPTS_FOLDER/setup-publish-variables.sh" "$ROOT_PATH"

# Ensure software
sudo apt update
sudo apt install zip -y

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

# Run HttPlaceholder integration tests
npm install newman --global
bash "$ROOT_PATH/test/exec-tests.sh"

# Run pre publish check
bash "$BUILDSCRIPTS_FOLDER/pre-publish-check.sh"

# Publish
echo "Type y to publish to GitHub releases."
read TYPE
if [ "$TYPE" = "y" ]; then
  GITHUB_API_KEY=$(cat $BUILD_METADATA_PATH/githubkey)
  COMMIT_HASH=$(git rev-parse HEAD)
  pwsh "$BUILDSCRIPTS_FOLDER/publish-to-github.ps1" -apiKey "$GITHUB_API_KEY" -distFolder "$DIST_PATH" -version "$VERSION" -commitHash "$COMMIT_HASH"
fi

echo "Type y to publish to Nuget."
read TYPE
if [ "$TYPE" = "y" ]; then
  GITHUB_API_KEY=$(cat $BUILD_METADATA_PATH/nugetkey)
  bash "$BUILDSCRIPTS_FOLDER/publish-nuget.sh" "$VERSION" "$GITHUB_API_KEY"
fi

echo "Type y to publish to Docker Hub."
read TYPE
if [ "$TYPE" = "y" ]; then
  DOCKER_USERNAME=$(cat $BUILD_METADATA_PATH/dockerusername)
  DOCKER_PASSWORD=$(cat $BUILD_METADATA_PATH/dockerpassword)
  bash "$BUILDSCRIPTS_FOLDER/publish-docker.sh" "$VERSION" "$DOCKER_USERNAME" "$DOCKER_PASSWORD"
fi