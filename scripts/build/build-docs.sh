#!/bin/bash
pip install mkdocs

DIR=$(pwd)
DIST_FOLDER="$DIR/dist"
if [ ! -d "$DIST_FOLDER" ]; then
  mkdir "$DIST_FOLDER"
fi

cd "$DIR/docs/httpl-docs"
python sync.py
mkdocs build
tar -czvf docs.tar.gz site
cp docs.tar.gz $DIST_FOLDER