#!/bin/bash
if [ "$1" = "" ]; then
    echo "Please provide version"
    exit 1
fi

# Set vars
VERSION=$1
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
DIST_DIR=$ROOT_DIR/dist
BIN_DIR=$ROOT_DIR/bin
INSTALL_SCRIPT_DIR=$DIR/installscripts/windows

# Publish application
cd src/HttPlaceholder
dotnet publish --configuration=release \
    --runtime=win-x64 \
    /p:PublishTrimmed=true \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION \
    -o $BIN_DIR
    
# Copy GUI to dist dir
cp -r $ROOT_DIR/gui/dist/. $BIN_DIR/gui

# Copy install scripts to dist dir
cp -r $INSTALL_SCRIPT_DIR/. $BIN_DIR

# Rename web.config
mv $BIN_DIR/web.config $BIN_DIR/_web.config

# Archive binaries
cd $BIN_DIR
zip -r httplaceholder_win-x64.zip .
cp httplaceholder_win-x64.zip $DIST_DIR