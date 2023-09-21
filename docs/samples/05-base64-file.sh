#!/bin/bash
# Read file from disk
curl --location --request GET 'http://localhost:5000/cat_file.jpg' -D-

# Read file from stub
curl --location --request GET 'http://localhost:5000/cat.jpg' -D-