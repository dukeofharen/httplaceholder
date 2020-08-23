#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
DOWNLOAD_DIR="$DIR/openapigenerator"
if [ ! -d "$DOWNLOAD_DIR" ]; then
    echo "Creating directory $DOWNLOAD_DIR"
    mkdir $DOWNLOAD_DIR
fi

JAR_NAME="openapi-generator-cli.jar"
JAR_PATH="$DOWNLOAD_DIR/$JAR_NAME"
if [ ! -f "$JAR_PATH" ]; then
    wget https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/4.3.1/openapi-generator-cli-4.3.1.jar -O $JAR_PATH
fi