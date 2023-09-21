#!/bin/bash
# Regular
curl --location --request GET "http://localhost:5000/dynamic-query.txt?response_text=This_is_the_response_text&response_header=This_is_the_response_header" -D-

# Encoded
curl --location --request GET "http://localhost:5000/dynamic-query.txt?response_text=This_is_the_response_text&response_header=This_is_the_response_header" -D-