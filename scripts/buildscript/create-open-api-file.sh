#!/bin/bash
set -e
set -u

ROOT_DIR=$1
DIST_DIR=$ROOT_DIR/dist
SWAGGER_GEN_DIR=$ROOT_DIR/src/HttPlaceholder.SwaggerGenerator

if [ ! -d "$DIST_DIR" ]; then
  mkdir "$DIST_DIR"
fi

# Run OpenAPI tool
cd $SWAGGER_GEN_DIR
dotnet run -c Release
cp $SWAGGER_GEN_DIR/bin/Release/net7.0/swagger.json $DIST_DIR