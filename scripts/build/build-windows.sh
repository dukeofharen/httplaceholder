#!/bin/bash
ROOT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
WIN_DIST_DIR=$ROOT_DIR/dist/windows
INSTALL_SCRIPT_DIR=$ROOT_DIR/scripts/build/installscripts/windows

mkdir $DIR/dist
mkdir $WIN_DIST_DIR

cd src/HttPlaceholder
dotnet publish $mainProjectFile --configuration=release --runtime=win-x64 /p:PublishTrimmed=true -o $WIN_DIST_DIR

cp -r $INSTALL_SCRIPT_DIR/. $WIN_DIST_DIR
mv $WIN_DIST_DIR/web.config $WIN_DIST_DIR/_web.config