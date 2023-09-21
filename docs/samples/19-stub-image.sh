#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# JPEG
curl --location --request GET "$HTTPL_ROOT_URL/stub-image.jpg" -D-

# Image with set font color
curl --location --request GET "$HTTPL_ROOT_URL/stub-image-font-color.png" -D-

# Image with very long text and word wrap
curl --location --request GET "$HTTPL_ROOT_URL/stub-image-with-very-long-text.png" -D-

# PNG
curl --location --request GET "$HTTPL_ROOT_URL/stub-image.png' -D-

# PNG with transparency
curl --location --request GET '$HTTPL_ROOT_URL/stub-image-transparency.png' -D-

# BMP
curl --location --request GET '$HTTPL_ROOT_URL/stub-image.bmp' -D-

# GIF
curl --location --request GET '$HTTPL_ROOT_URL/stub-image.gif' -D-