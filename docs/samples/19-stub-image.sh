#!/bin/bash
# JPEG
curl --location --request GET 'http://localhost:5000/stub-image.jpg' -D-

# Image with set font color
curl --location --request GET 'http://localhost:5000/stub-image-font-color.png' -D-

# Image with very long text and word wrap
curl --location --request GET 'http://localhost:5000/stub-image-with-very-long-text.png' -D-

# PNG
curl --location --request GET 'http://localhost:5000/stub-image.png' -D-

# PNG with transparency
curl --location --request GET 'http://localhost:5000/stub-image-transparency.png' -D-

# BMP
curl --location --request GET 'http://localhost:5000/stub-image.bmp' -D-

# GIF
curl --location --request GET 'http://localhost:5000/stub-image.gif' -D-