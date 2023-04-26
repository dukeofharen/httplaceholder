#!/bin/bash
set -e
set -u

ROOT_PATH="$1"
if [ "$ROOT_PATH" = "" ]; then
  echo "ROOT_PATH variable not set"
  exit 1  
fi

GUI_PATH="$ROOT_PATH/gui"
GUI_PUBLIC_PATH="$GUI_PATH/public"
GUI_DOCS_PATH="$GUI_PUBLIC_PATH/docs"
BUILT_DOCS_PATH="$ROOT_PATH/docs/httpl-docs/site"

if [ -d "$GUI_DOCS_PATH" ]; then
  rm -r "$GUI_DOCS_PATH"
fi

cp -r "$BUILT_DOCS_PATH/." "$GUI_DOCS_PATH"
cd "$GUI_PATH"
npm install
npm run build