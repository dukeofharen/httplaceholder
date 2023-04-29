#!/bin/bash
ROOT_PATH="$1"
if [ "$ROOT_PATH" = "" ]; then
  echo "ROOT_PATH variable not set"
  exit 1  
fi 

BUILD_METADATA_PATH="$ROOT_PATH/.build"

DOCKER_USERNAME_PATH="$BUILD_METADATA_PATH/dockerusername"
if [ ! -f "$DOCKER_USERNAME_PATH" ]; then
  echo "Fill in the Docker username"
  read DOCKER_USERNAME
  echo -n "$DOCKER_USERNAME" > "$DOCKER_USERNAME_PATH"
fi

DOCKER_PASSWORD_PATH="$BUILD_METADATA_PATH/dockerpassword"
if [ ! -f "$DOCKER_PASSWORD_PATH" ]; then
  echo "Fill in the Docker password"
  read DOCKER_PASSWORD
  echo -n "$DOCKER_PASSWORD" > "$DOCKER_PASSWORD_PATH"
fi

GITHUB_KEY_PATH="$BUILD_METADATA_PATH/githubkey"
if [ ! -f "$GITHUB_KEY_PATH" ]; then
  echo "Fill in the GitHub API key"
  read GITHUB_KEY
  echo -n "$GITHUB_KEY" > "$GITHUB_KEY_PATH"
fi

NUGET_KEY_PATH="$BUILD_METADATA_PATH/nugetkey"
if [ ! -f "$NUGET_KEY_PATH" ]; then
  echo "Fill in the Nuget API key"
  read NUGET_KEY
  echo -n "$NUGET_KEY" > "$NUGET_KEY_PATH"
fi