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

# Create dist dir
mkdir $DIST_DIR

# Pack tool
cd $ROOT_DIR/cs-client/src/HttPlaceholder.Client
dotnet pack -c Release \
    -o $DIST_DIR \
    /p:Version=$VERSION \
    /p:AssemblyVersion=$VERSION \
    /p:FileVersion=$VERSION \
    /p:Authors=Ducode \
    /p:Company=Ducode \
    /p:AssemblyTitle="HttPlaceholder NuGet Client" \
    /p:Description="HttPlaceholder NuGet Client" \
    /p:Copyright="2020 Ducode" \
    /p:RepositoryUrl="https://github.com/dukeofharen/httplaceholder" \
    /p:RepositoryType=git