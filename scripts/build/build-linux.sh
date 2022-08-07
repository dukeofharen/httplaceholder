#!/bin/bash
if [ "$1" = "" ]; then
    echo "Please provide version"
    exit 1
fi

set -e
set -u

# Set vars
VERSION=$1
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
DIST_DIR=$ROOT_DIR/dist
BIN_DIR=$ROOT_DIR/bin
INSTALL_SCRIPT_DIR=$DIR/installscripts/linux

# Create dist dir
if [ ! -d "$DIST_DIR" ]; then
  mkdir "$DIST_DIR"
fi

# Publish application
cd $ROOT_DIR/src/HttPlaceholder
dotnet publish --configuration=release \
    --runtime=linux-x64 \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION \
    -o $BIN_DIR
    
# Copy GUI to dist dir
cp -r $ROOT_DIR/gui/dist/. $BIN_DIR/gui

# Copy install scripts to dist dir
cp -r $INSTALL_SCRIPT_DIR/. $BIN_DIR

# Copy docs
cp -r $ROOT_DIR/docs $BIN_DIR

# Archive binaries
cd $BIN_DIR
tar -czvf $DIST_DIR/httplaceholder_linux-x64.tar.gz .
