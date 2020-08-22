#!/bin/bash
if [ "$1" = "" ]; then
    echo "Please provide version"
    exit 1
fi

# Set vars
VERSION=$1
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
LIN_DIST_DIR=$ROOT_DIR/dist/linux
INSTALL_SCRIPT_DIR=$DIR/installscripts/linux


# Publish application
cd src/HttPlaceholder
dotnet publish --configuration=release \
    --runtime=linux-x64 \
    /p:PublishTrimmed=true \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION \
    -o $LIN_DIST_DIR
    
# Copy GUI to dist dir
cp -r $ROOT_DIR/gui/dist/. $LIN_DIST_DIR/gui

# Copy install scripts to dist dir
cp -r $INSTALL_SCRIPT_DIR/. $LIN_DIST_DIR

# Remove web.config
rm $LIN_DIST_DIR/web.config

# Archive binaries
cd $LIN_DIST_DIR
tar -czvf httplaceholder_osx-x64.tar.gz .