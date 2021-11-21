#!/bin/bash
set -e
set -u

# This script is used to build the UIs in production mode and put them in the src folder so we can test the production UI builds locally.
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
SRC_DIR="$DIR/../../src/HttPlaceholder"

# New UI
NEW_UI_DIR="$DIR/../../gui"
NEW_UI_DIST_DIR="$NEW_UI_DIR/dist"
NEW_UI_SRC_DIR="$SRC_DIR/gui"
if [ -d "$NEW_UI_SRC_DIR" ]; then
  rm -r "$NEW_UI_SRC_DIR"
  mkdir "$NEW_UI_SRC_DIR"
  touch "$NEW_UI_SRC_DIR/.guiwillbeplacedhere"
fi

cd $NEW_UI_DIR
npm install
npm run build
cp -r $NEW_UI_DIST_DIR/. $NEW_UI_SRC_DIR