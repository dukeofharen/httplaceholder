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
INSTALL_SCRIPT_DIR=$DIR/installscripts/linux

# Create dist dir
mkdir $DIST_DIR

# Copy GUI to src dir
cp -r $ROOT_DIR/gui/dist/. $ROOT_DIR/src/HttPlaceholder/gui

# Pack tool
cd src/HttPlaceholder
sed -i 's/<PackAsTool>false<\/PackAsTool>/<PackAsTool>true<\/PackAsTool>/' HttPlaceholder.csproj
dotnet pack -c Tool \
    -o $BIN_DIR \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION