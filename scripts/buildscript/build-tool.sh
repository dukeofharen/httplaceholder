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

# Copy GUI to src dir
cp -r $ROOT_DIR/gui/dist/. $ROOT_DIR/src/HttPlaceholder/gui
PLACEHOLDER_PATH="$ROOT_DIR/src/HttPlaceholder/gui/.guiwillbeplacedhere"
if [ -f "$PLACEHOLDER_PATH" ]; then
  rm $ROOT_DIR/src/HttPlaceholder/gui/.guiwillbeplacedhere
fi

# Pack tool
cd $ROOT_DIR/src/HttPlaceholder
sed -i 's/<PackAsTool>false<\/PackAsTool>/<PackAsTool>true<\/PackAsTool>/' HttPlaceholder.csproj
dotnet pack -c Tool \
    -o $DIST_DIR \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION