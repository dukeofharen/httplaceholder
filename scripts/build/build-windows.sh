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
WIN_DIST_DIR=$DIST_DIR/windows
INSTALL_SCRIPT_DIR=$DIR/installscripts/windows

# Create dist dir
mkdir $DIR/dist
mkdir $WIN_DIST_DIR

# Publish application
cd src/HttPlaceholder
dotnet publish --configuration=release \
    --runtime=win-x64 \
    /p:PublishTrimmed=true \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION \
    -o $WIN_DIST_DIR
    
# Copy GUI to dist dir
cp -r $ROOT_DIR/gui/dist/. $WIN_DIST_DIR/gui

# Copy install scripts to dist dir
cp -r $INSTALL_SCRIPT_DIR/. $WIN_DIST_DIR

# Rename web.config
mv $WIN_DIST_DIR/web.config $WIN_DIST_DIR/_web.config