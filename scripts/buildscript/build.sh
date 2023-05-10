#!/bin/bash
set -e
set -u

ROOT_PATH="$1"
if [ "$ROOT_PATH" = "" ]; then
  echo "ROOT_PATH variable not set"
  exit 1  
fi

BUILDSCRIPTS_FOLDER="$ROOT_PATH/scripts/buildscript"
BUILD_METADATA_PATH="$ROOT_PATH/.build"
DIST_PATH="$ROOT_PATH/dist"
DOCKER_REPO_NAME="dukeofharen/httplaceholder"
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
NUGET_KEY=$(cat $BUILD_METADATA_PATH/nugetkey)
DOCKER_USERNAME=$(cat $BUILD_METADATA_PATH/dockerusername)
DOCKER_PASSWORD=$(cat $BUILD_METADATA_PATH/dockerpassword)
GITHUB_API_KEY=$(cat $BUILD_METADATA_PATH/githubkey)

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
bash "$BUILDSCRIPTS_FOLDER/build-docker.sh" "$VERSION" "$DOCKER_REPO_NAME"

# Run HttPlaceholder integration tests
npm install newman --global
bash "$ROOT_PATH/test/exec-tests.sh"

# Run pre publish check
bash "$BUILDSCRIPTS_FOLDER/pre-publish-check.sh"

# Publish
echo "Type y to publish to GitHub releases."
read TYPE
if [ "$TYPE" = "y" ]; then
  COMMIT_HASH=$(git rev-parse HEAD)
  pwsh "$BUILDSCRIPTS_FOLDER/publish-to-github.ps1" -apiKey "$GITHUB_API_KEY" -distFolder "$DIST_PATH" -version "$VERSION" -commitHash "$COMMIT_HASH"
fi

echo "Type y to publish the .NET client to Nuget."
read TYPE
if [ "$TYPE" = "y" ]; then
  bash "$BUILDSCRIPTS_FOLDER/publish-nuget.sh" "$VERSION" "$NUGET_KEY" HttPlaceholder.Client
fi

echo "Type y to publish the .NET tool to Nuget."
read TYPE
if [ "$TYPE" = "y" ]; then
  bash "$BUILDSCRIPTS_FOLDER/publish-nuget.sh" "$VERSION" "$NUGET_KEY" HttPlaceholder
fi

echo "Type y to publish to Docker Hub."
read TYPE
if [ "$TYPE" = "y" ]; then
  bash "$BUILDSCRIPTS_FOLDER/publish-docker.sh" "$VERSION" "$DOCKER_USERNAME" "$DOCKER_PASSWORD"
fi