#!/bin/bash
pip install mkdocs
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd "$DIR/../../docs/httpl-docs"
ls -la
mkdocs build
