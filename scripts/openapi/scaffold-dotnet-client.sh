#!/bin/bash
if [ "$1" = "" ]; then
    echo "Please provide the path to the swagger file"
    exit 1
fi

SWAGGER_PATH="$1"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
SOURCE_FOLDER="$DIR/../../cs-client"
cd $SOURCE_FOLDER
if [ -d "$SOURCE_FOLDER" ]; then
    echo "Deleting directory $SOURCE_FOLDER"
fi

echo "Creating directory $SOURCE_FOLDER"
mkdir $SOURCE_FOLDER

JAR_PATH="$DIR/openapigenerator/openapi-generator-cli.jar"
java -jar $JAR_PATH \
    generate -i $SWAGGER_PATH \
    -g csharp-netcore \
    --additional-properties=netCoreProjectFile=true,packageName=HttPlaceholder.Client