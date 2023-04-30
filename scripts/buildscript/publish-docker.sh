#!/bin/bash
VERSION="$1"
if [ "$VERSION" = "" ]; then
  echo "Version not set"
  exit 1
fi

DOCKER_USERNAME="$2"
if [ "$DOCKER_USERNAME" = "" ]; then
  echo "Docker username not set"
  exit 1
fi

DOCKER_PASSWORD="$3"
if [ "$DOCKER_PASSWORD" = "" ]; then
  echo "Docker password not set"
  exit 1
fi

echo "$DOCKER_PASSWORD" | docker login -u "$DOCKER_USERNAME" --password-stdin

REPO_NAME="dukeofharen/httplaceholder"
docker build -t ${REPO_NAME}:${VERSION} .
docker tag ${REPO_NAME}:${VERSION} ${REPO_NAME}:latest
docker push ${REPO_NAME}:${VERSION}
docker push ${REPO_NAME}:latest