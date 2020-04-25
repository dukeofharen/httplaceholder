#!/bin/bash
if [[ "$1" = "" ]]; then
    echo "Tag not set"
    exit 1
fi

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
TAG="$1"
REPO_NAME="dukeofharen/httplaceholder"
cd "$DIR/../.."
docker build -t ${REPO_NAME}:${TAG} .
docker tag ${REPO_NAME}:${TAG} ${REPO_NAME}:latest
docker push ${REPO_NAME}:${TAG}
docker push ${REPO_NAME}:latest