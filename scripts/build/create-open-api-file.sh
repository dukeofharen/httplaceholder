#!/bin/bash
set -e
set -u

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
DIST_DIR=$ROOT_DIR/dist
SWAGGER_GEN_DIR=$ROOT_DIR/src/HttPlaceholder.SwaggerGenerator

mkdir $DIST_DIR

# Run OpenAPI tool
cd $SWAGGER_GEN_DIR
dotnet run -c Release
cp $SWAGGER_GEN_DIR/bin/Release/net5.0/swagger.json $DIST_DIR