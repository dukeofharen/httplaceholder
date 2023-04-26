#!/bin/bash
set -e
set -u

ROOT_PATH="$1"
if [ "$ROOT_PATH" = "" ]; then
  echo "ROOT_PATH variable not set"
  exit 1  
fi

SRC_PATH="$ROOT_PATH/src"
cd $SRC_PATH
dotnet test