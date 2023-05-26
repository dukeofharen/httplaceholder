#!/bin/bash
VERSION="$1"
if [ "$VERSION" = "" ]; then
  echo "Version not set"
  exit 1
fi

DOCKER_REPO_NAME="$2"
if [ "$DOCKER_REPO_NAME" = "" ]; then
  echo "Docker repo name not set"
  exit 1
fi

docker build -t ${DOCKER_REPO_NAME}:${VERSION} .
docker tag ${DOCKER_REPO_NAME}:${VERSION} ${DOCKER_REPO_NAME}:latest
