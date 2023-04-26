#!/bin/bash
set -e
set -u

# TODO pack docs and put in dist directory
ROOT_PATH="$1"
if [ "$ROOT_PATH" = "" ]; then
  echo "ROOT_PATH variable not set"
  exit 1  
fi

echo "Building documentation"

pip install mkdocs

DIST_FOLDER="$ROOT_PATH/dist"
cd "$ROOT_PATH/docs/httpl-docs"
python sync.py
mkdocs build
tar -czvf docs.tar.gz site
cp docs.tar.gz $DIST_FOLDER
