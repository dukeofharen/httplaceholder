#!/bin/bash
if [ "$1" = "" ]; then
    echo "Please provide version"
    exit 1
fi

# Set vars
VERSION=$1
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
MAC_DIST_DIR=$ROOT_DIR/dist/mac
INSTALL_SCRIPT_DIR=$DIR/installscripts/mac


# Publish application
cd src/HttPlaceholder
dotnet publish --configuration=release \
    --runtime=osx-x64 \
    /p:PublishTrimmed=true \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION \
    -o $MAC_DIST_DIR
    
# Copy GUI to dist dir
cp -r $ROOT_DIR/gui/dist/. $MAC_DIST_DIR/gui

# Copy install scripts to dist dir
cp -r $INSTALL_SCRIPT_DIR/. $MAC_DIST_DIR

# Remove web.config
rm $MAC_DIST_DIR/web.config

# Archive binaries
cd $MAC_DIST_DIR
tar -czvf httplaceholder_linux-x64.tar.gz .