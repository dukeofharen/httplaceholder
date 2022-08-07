#!/bin/bash
pip install mkdocs
cd "docs/httpl-docs"
python sync.py
mkdocs build
