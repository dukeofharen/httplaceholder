#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
SOURCE_FOLDER="$DIR/../../cs-client"
SWAGGER_PATH="/tmp/swagger.json"
SWAGGERGEN_PATH="$DIR/../../src/HttPlaceholder.SwaggerGenerator"
cd $SWAGGERGEN_PATH
dotnet run "$SWAGGER_PATH"

if [ ! -d "$SOURCE_FOLDER" ]; then
    echo "Creating directory $SOURCE_FOLDER"
    mkdir $SOURCE_FOLDER
fi

cd $SOURCE_FOLDER

JAR_PATH="$DIR/openapigenerator/openapi-generator-cli.jar"
java -jar $JAR_PATH \
    generate -i $SWAGGER_PATH \
    -g csharp-netcore \
    --additional-properties=netCoreProjectFile=true,packageName=HttPlaceholder.Client
    
find . -type f -regex '.*\.\(md\|cs\)' -print0 | while read -d $'\0' FILE
do
    echo "Patching file $FILE"
    sed -i 's/OneOf//g' $FILE
    sed -i 's/AnyType/object/g' $FILE
done