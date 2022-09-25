#!/bin/bash
set -e
set -u

npm run build-site-dev
#npm run build-site-prod

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR="$DIR/../.."
DOCS_ROOT_DIR="$ROOT_DIR/docs/httpl-docs"
DOCS_DIST_DIR="$DOCS_ROOT_DIR/site"
SITE_DIST_DIR="$DIR/../dist"

# Build documentation
pip install mkdocs

cd $DOCS_ROOT_DIR
python sync.py
mkdocs build

# Copy built docs to website folder
mkdir "$SITE_DIST_DIR/docs"
cp -r "$DOCS_DIST_DIR/." "$SITE_DIST_DIR/docs"