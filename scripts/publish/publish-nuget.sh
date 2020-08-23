#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
DIST_DIR=$ROOT_DIR/dist
dotnet nuget push $DIST_DIR/*.nupkg -k $NUGET_API_KEY -s https://api.nuget.org/v3/index.json