#!/bin/bash
if [ "$1" = "" ]; then
    echo "Please provide version"
    exit 1
fi

# Set vars
VERSION=$1
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
WIN_DIST_DIR=$ROOT_DIR/dist/windows
INSTALL_SCRIPT_DIR=$DIR/installscripts/windows


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

# Archive binaries
cd $WIN_DIST_DIR
zip httplaceholder_win-x64.zip -r .