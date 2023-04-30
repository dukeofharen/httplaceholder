#!/bin/bash
if [ "$1" = "" ]; then
    echo "Please provide version"
    exit 1
fi

set -e
set -u

# Set vars
VERSION=$1
ROOT_DIR=$2
DIST_DIR=$ROOT_DIR/dist

# Create dist dir
if [ ! -d "$DIST_DIR" ]; then
  mkdir $DIST_DIR
fi

# Pack tool
cd $ROOT_DIR/src/HttPlaceholder.Client
dotnet pack -c Release \
    -o $DIST_DIR \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION
