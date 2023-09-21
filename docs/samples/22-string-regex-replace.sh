#!/bin/bash
# String replace
curl --location --request GET 'http://localhost:5000/string-replace' -D-

# Regex replace
curl --location --request GET 'http://localhost:5000/regex-replace?queryString=Bassie' -D-

# String and regex replace
curl --location --request GET 'http://localhost:5000/string-and-regex-replace' -D-