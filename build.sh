#!/bin/bash
ROOT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
BUILD_SCRIPT_PATH="$ROOT_PATH/scripts/buildscript/build.sh"
chmod +x $BUILD_SCRIPT_PATH
bash $BUILD_SCRIPT_PATH "$ROOT_PATH"