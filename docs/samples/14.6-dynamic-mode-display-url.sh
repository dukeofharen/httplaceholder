#!/bin/bash
# Regular
curl --location --request GET "http://localhost:5000/dynamic-display-url.txt?testvar=value1" -D-

# Regex
curl --location --request GET 'http://localhost:5000/dynamic-display-url-regex/users/451/orders' -D-